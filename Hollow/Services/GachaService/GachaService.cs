using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Gacha;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Models.Pages.SignalSearch;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Serilog;

namespace Hollow.Services.GachaService;

public partial class GachaService(IConfigurationService configurationService, HttpClient httpClient, IGameService gameService) : IGachaService
{
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }

    public GachaRecords MergeGachaRecordsFromImport(GachaRecords fileJson, ImportItem[] selectedImportItems, Dictionary<string, HakushItemModel> itemsMetadata)
    {
        var currentGachaProfiles = GachaRecordProfileDictionary ?? new Dictionary<string, GachaRecordProfile>();
        foreach (var selectedImportItem in selectedImportItems)
        {
            var completedGachaItems = CompleteGachaItems(fileJson.Profiles.First(profile => profile.Uid == selectedImportItem.Uid).List, itemsMetadata);
                    
            if(currentGachaProfiles.TryGetValue(selectedImportItem.Uid, out var currentGachaProfile))
            {
                var mergedList = completedGachaItems
                    .Concat(currentGachaProfile.List)
                    .GroupBy(item => item.Id)
                    .Select(group => group.First())
                    .OrderByDescending(item => item.Id)
                    .ToList();
                
                currentGachaProfiles.Remove(selectedImportItem.Uid);
                currentGachaProfiles.Add(selectedImportItem.Uid, new GachaRecordProfile
                {
                    Uid = selectedImportItem.Uid,
                    Timezone = selectedImportItem.I18NTimezone.ToTimeZoneFromUtcPrefix(),
                    List = mergedList
                });
            }
            else
            {
                currentGachaProfiles.Add(selectedImportItem.Uid, new GachaRecordProfile
                {
                    Uid = selectedImportItem.Uid,
                    Timezone = selectedImportItem.I18NTimezone.ToTimeZoneFromUtcPrefix(),
                    List = completedGachaItems
                });
            }
        }
        
        return new GachaRecords
        {
            Info =
            {
                ExportApp = "Hollow",
                ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
            }, 
            Profiles = currentGachaProfiles.Values.ToList()
        };
    }

    private static List<GachaItem> CompleteGachaItems(List<GachaItem> gachaItems, Dictionary<string, HakushItemModel> metadata)
    {
        foreach (var gachaItem in gachaItems)
        {
            var metadataItem = metadata.GetValueOrDefault(gachaItem.ItemId);
            if (metadataItem is null) continue;
            
            gachaItem.Name = metadataItem.ChineseName;
            gachaItem.RankType = metadataItem.RankType.ToString() ?? throw new ArgumentNullException($"Unknown Character: {gachaItem.Name}");
            gachaItem.ItemType = gachaItem.ItemType;
        }
        return gachaItems;
    }
    
    public async Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary()
    {
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
    
    private string GetLatestId(string uid, string gachaType)
    {
        if(GachaRecordProfileDictionary is null || !GachaRecordProfileDictionary.TryGetValue(uid, out var record))
        {
            return "0";
        }
        return record.List.Find(item => item.GachaType == gachaType)?.Id ?? "0";
    }
    
    private static (bool, List<GachaItem>) OmitExistedRecords(string id, List<GachaItem> gachaItems)
    {
        var endTimeIndex = gachaItems.FindIndex(0, item => item.Id == id);
        return endTimeIndex != -1 ? (true, gachaItems[..endTimeIndex]) : (false, gachaItems);
    }
    
    private const string GachaLogChinaUrl = "https://public-operation-nap.mihoyo.com/common/gacha_record/api/getGachaLog?authkey_ver=1&authkey={0}&lang=zh-cn&game_biz=nap_cn&size=20&real_gacha_type={1}&end_id=";
    private const string GachaLogGlobalUrl = "https://public-operation-nap-sg.hoyoverse.com/common/gacha_record/api/getGachaLog?authkey_ver=1&authkey={0}&lang=zh-cn&game_biz=nap_global&size=20&real_gacha_type={1}&end_id=";
    private readonly int[] _gachaTypes = [1, 2, 3, 5]; // 1 - standard, 2 - exclusive, 3 - w-engine, 5 - bangboo

    public async Task<Response<GachaRecords>> GetGachaRecords(GachaUrlData gachaUrlData, IProgress<Response<string>> progress,
        GameServer gameServer)
    {
        Log.Information("[GachaService] Start fetching gacha records");
        var gachaRecords = new GachaRecords
        {
            Profiles = GachaRecordProfileDictionary?.Values.ToList() ?? [],
        };
        var uid = "";
        var fetchRecordsCount = 0;
        var newRecordsCount = 0;

        var authKey = gachaUrlData.AuthKey;
        var regionTimeZone = gachaUrlData.Region switch
        {
            "prod_gf_cn" => 8,
            "prod_gf_us" => -5,
            "prod_gf_jp" => 9,
            "prod_gf_eu" => 1,
            _ => 8
        };
        var gachaLogUrl = gameServer switch
        {
            GameServer.China => GachaLogChinaUrl,
            GameServer.Global => GachaLogGlobalUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(gameServer), gameServer, null)
        };
        
        var targetProfile = new GachaRecordProfile
        {
            Timezone = regionTimeZone
        };

        foreach (var gachaType in _gachaTypes)
        {
            var nthPage = 1;
            var pageEndId = "0";
            while (true)
            {
                var pageUrl = string.Format(gachaLogUrl, authKey, gachaType);
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
                    var time = GetLatestId(uid, gachaType.ToString());
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
        var existedProfile = gachaRecords.Profiles.Find(item => item.Uid == uid);
        if (existedProfile != null)
        {
            gachaRecords.Profiles.Remove(existedProfile);
        }
        gachaRecords.Profiles.Add(targetProfile);
        
        gachaRecords.Info.ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        gachaRecords.Info.ExportApp = "Hollow";
        
        progress.Report(new Response<string>(true, $"success {fetchRecordsCount} {newRecordsCount}"));
        Log.Information("[GachaService] Fetched {0} new records", newRecordsCount);

        return new Response<GachaRecords>(true) {Data = gachaRecords};
    }

    public async Task<bool> IsAuthKeyValid(string authKey)
    {
        var gachaLogUrl = gameService.GameBiz switch
        {
            GameServer.China => GachaLogChinaUrl,
            GameServer.Global => GachaLogGlobalUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(gameService.GameBiz),  gameService.GameBiz, null)
        };
        var firstPage = await httpClient.GetAsync(string.Format(gachaLogUrl, authKey, _gachaTypes[0]));
        var firstPageContent = await firstPage.Content.ReadAsStringAsync();
        return !firstPageContent.Contains("\"data\":null");
    }
    
    public Response<GachaUrlData> GetGachaUrlData()
    {
        var gameDirectory = configurationService.AppConfig.Game.Directory;
        if (string.IsNullOrWhiteSpace(gameDirectory))
        {
            Log.Error("[GachaService] Game directory not found");
            return new Response<GachaUrlData>(false, Lang.Toast_UnknownGameDirectory_Message);
        }
        
        var webCachesPath = Path.Combine(gameDirectory, "ZenlessZoneZero_Data", "webCaches");
        if (!Directory.Exists(webCachesPath))
        {
            Log.Error("[GachaService] Web caches not found");
            return new Response<GachaUrlData>(false, Lang.Toast_WebCachesNotFound_Message);
        }
        
        var webCachesVersionPath = Directory.GetDirectories(webCachesPath).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(webCachesVersionPath))
        {
            Log.Error("[GachaService] Web caches version not found");
            return new Response<GachaUrlData>(false, Lang.Toast_WebCachesNotFound_Message);
        }
        
        var dataPath = Path.Combine(webCachesVersionPath, "Cache", "Cache_Data", "data_2");
        if (!File.Exists(dataPath))
        {
            Log.Error("[GachaService] Data file not found");
            return new Response<GachaUrlData>(false, Lang.Toast_WebCachesNotFound_Message);
        }
        
        return GetGachaUrlDataFromFile(dataPath, gameService.GameBiz);
    }

    public Response<GachaUrlData> GetGachaUrlDataFromUrl(string url)
    {
        if (!GachaLogChinaUrlRegex().IsMatch(url) && !GachaLogGlobalUrlRegex().IsMatch(url))
        {
            Log.Error("[GachaService] Invalid URL: {url}", url);
            return new Response<GachaUrlData>(false, Lang.Toast_InvalidUrl_Message);
        }
        try
        {
            var targetAuthKey = url.Split("&authkey=")[1].Split("&")[0];
            var targetRegion = url.Split("&region=")[1].Split("&")[0];
            Log.Information("[GachaService] Get authKey from URL: {0}", targetAuthKey);
            return new Response<GachaUrlData> (true) {Data = new GachaUrlData {AuthKey = targetAuthKey, Region = targetRegion}};
        }
        catch (Exception)
        {
            return new Response<GachaUrlData>(false, Lang.Toast_InvalidUrl_Message);
        }
    }

    private Response<GachaUrlData> GetGachaUrlDataFromFile(string dataPath, GameServer gameServer)
    {
        using var fileStream = File.Open(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(fileStream);
        var data = reader.ReadToEnd();
        var gachaLogUrls = gameServer switch
        {
            GameServer.China => GachaLogChinaUrlRegex().Matches(data),
            GameServer.Global => GachaLogGlobalUrlRegex().Matches(data),
            _ => default
        };
        
        if (gachaLogUrls == null || gachaLogUrls.Count == 0)
        {
            return new Response<GachaUrlData>(false, Lang.Toast_AuthKeyNotFound_Message);
        }
        var targetAuthKey = gachaLogUrls[^1].Value.Split("&authkey=")[1].Split("&")[0];
        var targetRegion = gachaLogUrls[^1].Value.Split("&region=")[1].Split("&")[0];
        Log.Information("[GachaService] Get authKey from WebCaches: {0}", targetAuthKey);
        return new Response<GachaUrlData>(true) { Data = new GachaUrlData {AuthKey = targetAuthKey, Region = targetRegion}  };
    }

    [GeneratedRegex(@"https://webstatic.mihoyo.com/nap/event/e20230424gacha/index.html[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]")]
    private static partial Regex GachaLogChinaUrlRegex();
    
    [GeneratedRegex(@"https://gs.hoyoverse.com/nap/event/e20230424gacha/index.html[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]")]
    private static partial Regex GachaLogGlobalUrlRegex();
}