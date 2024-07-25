using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Gacha;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Services.ConfigurationService;
using Serilog;

namespace Hollow.Services.GachaService;

public partial class GachaService(IConfigurationService configurationService, HttpClient httpClient) : IGachaService
{
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }
    
    public async Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary()
    {
        //TODO: UIGF 4.0 Schema + Strict Json Validation
        GachaRecords gachaRecords;
        if(!File.Exists(AppInfo.GachaRecordPath))
        {
            gachaRecords = new GachaRecords();
            await File.WriteAllTextAsync(AppInfo.GachaRecordPath, JsonSerializer.Serialize(gachaRecords, HollowJsonSerializer.Options));
            Log.Information("[GachaService] Gacha record file not found, created new one");
        }
        else
        {
            gachaRecords = JsonSerializer.Deserialize<GachaRecords>(await File.ReadAllTextAsync(AppInfo.GachaRecordPath))!;
            Log.Information("[GachaService] Gacha record file loaded");
        }

        var gachaRecordProfileDictionary = gachaRecords.Profiles.ToDictionary(item => item.Uid, item => item);
        foreach (var profile in gachaRecordProfileDictionary.Keys)
        {
            gachaRecordProfileDictionary[profile].List = gachaRecordProfileDictionary[profile].List.OrderByDescending(item => item.Id).ToList();
        }

        GachaRecordProfileDictionary = gachaRecordProfileDictionary;
        
        Log.Information("[GachaService] Found {0} gacha record profiles", GachaRecordProfileDictionary.Count);
        return GachaRecordProfileDictionary;
    }
    
    private string GetLatestTime(string uid, string gachaType)
    {
        if(GachaRecordProfileDictionary is null || !GachaRecordProfileDictionary.TryGetValue(uid, out var record))
        {
            return "0";
        }
        return record.List.Find(item => item.GachaType == gachaType)?.Time ?? "0";
    }
    
    private static (bool, List<GachaItem>) OmitExistedRecords(string time, List<GachaItem> gachaItems)
    {
        var endTimeIndex = gachaItems.FindIndex(0, item => item.Time == time);
        return endTimeIndex != -1 ? (true, gachaItems[..endTimeIndex]) : (false, gachaItems);
    }
    
    private const string GachaLogUrl = "https://public-operation-nap.mihoyo.com/common/gacha_record/api/getGachaLog?authkey_ver=1&authkey={0}&lang=zh-cn&game_biz=nap_cn&size=20&real_gacha_type={1}&end_id=";
    private readonly int[] _gachaTypes = [1, 2, 3, 5]; // 1 - standard, 2 - exclusive, 3 - w-engine, 5 - bangboo
    
    public async Task<Response<GachaRecords>> GetGachaRecords(string authKey, IProgress<Response<string>> progress)
    {
        Log.Information("[GachaService] Start fetching gacha records");
        var gachaRecords = new GachaRecords();
        var targetProfile = new GachaRecordProfile();
        var uid = "";
        var isCompletionMode = false;
        var fetchRecordsCount = 0;
        var newRecordsCount = 0;

        foreach (var gachaType in _gachaTypes)
        {
            var nthPage = 1;
            var pageEndId = "0";
            while (true)
            {
                var pageUrl = string.Format(GachaLogUrl, authKey, gachaType);
                if (nthPage > 1)
                {
                    pageUrl += pageEndId;
                }
                
                var page = await httpClient.GetAsync(pageUrl);
                var pageContent = await page.Content.ReadAsStringAsync();
                var pageData = JsonSerializer.Deserialize<GachaPage>(pageContent);
                if (pageData is null || pageData.Data!.List.Count == 0)
                {
                    break;
                }
                var pageDataList = pageData.Data!.List;
                
                fetchRecordsCount += pageDataList.Count;
                if (nthPage == 1)
                {
                    uid = pageDataList[0].Uid!;
                    targetProfile.Uid = uid;
                }

                foreach (var pageDataListItem in pageDataList)
                {
                    pageDataListItem.Uid = null;
                }
                
                // If UID exists in records, into completion
                if(GachaRecordProfileDictionary is not null && GachaRecordProfileDictionary.TryGetValue(uid, out var value) && !configurationService.AppConfig.Records.FullUpdate)
                {
                    isCompletionMode = true;
                    var time = GetLatestTime(uid, gachaType.ToString());
                    var omitted = OmitExistedRecords(time, pageDataList);
                    if (omitted.Item1)
                    {
                        newRecordsCount += omitted.Item2.Count;
                        progress.Report(new Response<string>(true, "progress") { Data = $"{string.Join('^', omitted.Item2.Select(x => x.Name))}^{uid}^{gachaType}^{nthPage}"});
                        
                        var targetExistedGachaRecords = value.List.Where(item => item.GachaType == gachaType.ToString());
                        targetProfile.List.AddRange(omitted.Item2);
                        targetProfile.List.AddRange(targetExistedGachaRecords);
                        break;
                    }
                    pageDataList = omitted.Item2;
                }
                
                newRecordsCount += pageDataList.Count;
                targetProfile.List.AddRange(pageDataList);

                var dataItemsName= pageData.Data.List.Select(x => x.Name).ToArray();
                progress.Report(new Response<string>(true, "progress") { Data = $"{string.Join('^', dataItemsName)}^{uid}^{gachaType}^{nthPage}"});
                Log.Information("[GachaService] Fetched Records ({0} Items | Page {1} | Gacha Type {2} | UID {3})", dataItemsName.Length, nthPage, gachaType, uid);
                
                pageEndId = pageData.Data.List[^1].Id;
                nthPage++;
                await Task.Delay(TimeSpan.FromMilliseconds(400));
            }
            
            await Task.Delay(TimeSpan.FromMilliseconds(400));
        }

        if(isCompletionMode)
        {
            gachaRecords.Profiles.Remove(GachaRecordProfileDictionary![uid]);
        }
        gachaRecords.Profiles.Add(targetProfile);
        
        gachaRecords.Info.ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        gachaRecords.Info.ExportAppVersion = AppInfo.AppVersion;
        progress.Report(new Response<string>(true, $"success {fetchRecordsCount} {newRecordsCount}"));
        Log.Information("[GachaService] Fetched {0} new records", newRecordsCount);

        return new Response<GachaRecords>(true) {Data = gachaRecords};
    }

    public async Task<bool> IsAuthKeyValid(string authKey)
    {
        var firstPage = await httpClient.GetAsync(string.Format(GachaLogUrl, authKey, _gachaTypes[0]));
        var firstPageContent = await firstPage.Content.ReadAsStringAsync();
        return !firstPageContent.Contains("\"data\":null");
    }
    
    public Response<string> GetAuthKey()
    {
        var gameDirectory = configurationService.AppConfig.Game.Directory;
        if (string.IsNullOrWhiteSpace(gameDirectory))
        {
            Log.Error("[GachaService] Game directory not found");
            return new Response<string>(false, Lang.Toast_UnknownGameDirectory_Message);
        }
        
        var webCachesPath = Path.Combine(gameDirectory, "ZenlessZoneZero_Data", "webCaches");
        if (!Directory.Exists(webCachesPath))
        {
            Log.Error("[GachaService] Web caches not found");
            return new Response<string>(false, Lang.Toast_WebCachesNotFound_Message);
        }
        
        var webCachesVersionPath = Directory.GetDirectories(webCachesPath).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(webCachesVersionPath))
        {
            Log.Error("[GachaService] Web caches version not found");
            return new Response<string>(false, Lang.Toast_WebCachesNotFound_Message);
        }
        
        var dataPath = Path.Combine(webCachesVersionPath, "Cache", "Cache_Data", "data_2");
        if (!File.Exists(dataPath))
        {
            Log.Error("[GachaService] Data file not found");
            return new Response<string>(false, Lang.Toast_WebCachesNotFound_Message);
        }

        var authKey = GetAuthKeyFromFile(dataPath);
        if (!authKey.IsSuccess)
        {
            return authKey;
        }

        var targetGachaLogUrl = authKey.Data.Split("&authkey=")[1].Split("&")[0];
        Log.Information("[GachaService] Get authKey from WebCaches: {0}", targetGachaLogUrl);
        return new Response<string> (true) {Data = targetGachaLogUrl};
    }

    public Response<string> GetAuthKeyFromUrl(string url)
    {
        if (!GachaLogUrlRegex().IsMatch(url) && !url.StartsWith(_gachaLogClientUrl))
        {
            Log.Error("[GachaService] Invalid URL: {url}", url);
            return new Response<string>(false, Lang.Toast_InvalidUrl_Message);
        }

        try
        {
            var targetGachaLogUrl = url.Split("&authkey=")[1].Split("&")[0];
            Log.Information("[GachaService] Get authKey from URL: {0}", targetGachaLogUrl);
            return new Response<string> (true) {Data = targetGachaLogUrl};
        }
        catch (Exception)
        {
            return new Response<string>(false, Lang.Toast_InvalidUrl_Message);
        }
    }

    private Response<string> GetAuthKeyFromFile(string dataPath)
    {
        using var fileStream = File.Open(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(fileStream);
        var data = reader.ReadToEnd();
        var gachaLogUrls = GachaLogUrlRegex().Matches(data);
        if (gachaLogUrls.Count == 0)
        {
            return new Response<string>(false, Lang.Toast_AuthKeyNotFound_Message);
        }

        return new Response<string>(true) { Data = gachaLogUrls[^1].Value };
    }

    [GeneratedRegex(@"https://public-operation-nap.mihoyo.com/common/gacha_record/api/getGachaLog[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]end_id=")]
    private static partial Regex GachaLogUrlRegex();

    private readonly string _gachaLogClientUrl = "https://webstatic.mihoyo.com/nap/event/e20230424gacha/index.html";
}