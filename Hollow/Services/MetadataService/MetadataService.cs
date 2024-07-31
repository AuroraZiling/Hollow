using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Abstractions.Models.HttpContrasts.Hakush.Proceed;
using Hollow.Helpers;

namespace Hollow.Services.MetadataService;

public class MetadataService(HttpClient httpClient): IMetadataService
{
    public Dictionary<string, ProceedHakushItemModel>? ItemsMetadata { get; set; }
    private const string ItemMetadataIconBaseUrl = "https://api.hakush.in/zzz/UI";

    private static string ProcessIconUrl(string iconUrl, string itemType)
    {
        if (string.IsNullOrEmpty(iconUrl))
        {
            return "";
        }
        return itemType switch
        {
            "代理人" => $"{ItemMetadataIconBaseUrl}/IconRoleSelect{iconUrl[8..]}.webp",
            "音擎" or "邦布" => $"{ItemMetadataIconBaseUrl}/{iconUrl.Split('/')[^1][..^4]}.webp",
            _ => iconUrl
        };
    }
    
    public async Task LoadItemMetadata(IProgress<Response<string>> progress, bool force = false)
    {
        var urls = new Dictionary<string, string> {
            { "代理人" ,"https://api.hakush.in/zzz/data/character.json" }, 
            { "音擎" ,"https://api.hakush.in/zzz/data/weapon.json" }, 
            { "邦布" ,"https://api.hakush.in/zzz/data/bangboo.json" }, 
        };
        var itemMetadataPath = Path.Combine(AppInfo.MetadataDir, "item.json");
        if (!File.Exists(itemMetadataPath))
        {
            progress.Report(new Response<string>(await DownloadItemMetadata()));
        }
        else
        {
            var metadataCreationTime = ((DateTimeOffset)new FileInfo(itemMetadataPath).CreationTime).ToUnixTimeSeconds();
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (now - metadataCreationTime >= 1209600 || force)
            {
                progress.Report(new Response<string>(await DownloadItemMetadata()));
            }
        }
        ItemsMetadata = JsonSerializer.Deserialize<Dictionary<string, ProceedHakushItemModel>>(await File.ReadAllTextAsync(itemMetadataPath), HollowJsonSerializer.Options);
        return;

        async Task<bool> DownloadItemMetadata()
        {
            try
            {
                var items = new Dictionary<string, ProceedHakushItemModel>();
                foreach (var url in urls)
                {
                    var metadata = await httpClient.GetStringAsync(url.Value);
                    var itemsInType = JsonSerializer.Deserialize<Dictionary<string, HakushItemModel>>(metadata, HollowJsonSerializer.Options)!;
                    
                    foreach (var (key, value) in itemsInType)
                    {
                        items[key] = new ProceedHakushItemModel
                        {
                            ChineseName = value.ChineseName,
                            EnglishName = value.EnglishName,
                            GachaType = value.GachaType,
                            RankType = value.RankType,
                            ItemType = url.Key,
                            Icon = ProcessIconUrl(value.Icon, url.Key),
                            IsCompleted = value.Icon != "" && value.ChineseName != "..." && value.EnglishName != "..." && value is { GachaType: not null, RankType: not null }
                        };
                    }
                }

                if (File.Exists(itemMetadataPath))
                {
                    File.Delete(itemMetadataPath);
                }
                await File.WriteAllTextAsync(itemMetadataPath, JsonSerializer.Serialize(items, HollowJsonSerializer.Options));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}