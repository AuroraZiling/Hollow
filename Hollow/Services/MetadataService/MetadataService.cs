using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hollow.Abstractions.Enums.Hakush;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Abstractions.Models.HttpContrasts.Hakush.Intermediate;

namespace Hollow.Services.MetadataService;

public class MetadataService(HttpClient httpClient): IMetadataService
{
    public Dictionary<string, HakushItemModel>? ItemsMetadata { get; set; }
    private const string ItemMetadataIconBaseUrl = "https://api.hakush.in/zzz/UI";

    private static string ProcessIconUrl(string iconUrl, HakushItemType itemType)
    {
        if (string.IsNullOrEmpty(iconUrl))
        {
            return "";
        }
        return itemType switch
        {
            HakushItemType.Character => $"{ItemMetadataIconBaseUrl}/IconRoleSelect{iconUrl[8..]}.webp",
            HakushItemType.Weapon or HakushItemType.Bangboo or HakushItemType.Equipment => $"{ItemMetadataIconBaseUrl}/{iconUrl.Split('/')[^1][..^4]}.webp",
            _ => iconUrl
        };
    }
    
    public async Task LoadItemMetadata(IProgress<Response<string>> progress, bool force = false)
    {
        var urls = new Dictionary<HakushItemType, string> {
            { HakushItemType.Character ,"https://api.hakush.in/zzz/data/character.json" }, 
            { HakushItemType.Weapon ,"https://api.hakush.in/zzz/data/weapon.json" }, 
            { HakushItemType.Bangboo ,"https://api.hakush.in/zzz/data/bangboo.json" }, 
            { HakushItemType.Equipment, "https://api.hakush.in/zzz/data/equipment.json"}
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
        ItemsMetadata = JsonSerializer.Deserialize<Dictionary<string, HakushItemModel>>(await File.ReadAllTextAsync(itemMetadataPath), HollowJsonSerializer.Options);
        return;

        async Task<bool> DownloadItemMetadata()
        {
            try
            {
                var items = new Dictionary<string, HakushItemModel>();
                foreach (var url in urls)
                {
                    var metadata = await httpClient.GetStringAsync(url.Value);

                    if (url.Key != HakushItemType.Equipment)
                    {
                        var itemsInType = JsonSerializer.Deserialize<Dictionary<string, HakushItemModel>>(metadata, HollowJsonSerializer.Options)!;
                    
                        foreach (var (key, value) in itemsInType)
                        {
                            var itemModel = new HakushItemModel
                            {
                                ChineseName = value.ChineseName,
                                EnglishName = value.EnglishName,
                                ItemPropertyType = value.ItemPropertyType,
                                RankType = value.RankType,
                                ItemType = url.Key,
                                Icon = ProcessIconUrl(value.Icon, url.Key),
                                IsCompleted = value.Icon != "" && value.ChineseName != "..." && value.EnglishName != "..." && value is { ItemPropertyType: not null, RankType: not null }
                            };

                            if (url.Key == HakushItemType.Character && itemModel.IsCompleted)
                            {
                                itemModel.CharacterTypeIconUrl = value.ItemPropertyType switch
                                {
                                    1 => $"{ItemMetadataIconBaseUrl}/IconAttack.webp",
                                    2 => $"{ItemMetadataIconBaseUrl}/IconStun.webp",
                                    3 => $"{ItemMetadataIconBaseUrl}/IconAnomaly.webp",
                                    4 => $"{ItemMetadataIconBaseUrl}/IconSupport.webp",
                                    5 => $"{ItemMetadataIconBaseUrl}/IconDefense.webp",
                                    _ => ""
                                };
                            }

                            items[key] = itemModel;
                        }
                    }
                    else
                    {
                        var equipmentInType = JsonSerializer.Deserialize<Dictionary<string, HakushEquipmentModel>>(metadata, HollowJsonSerializer.Options)!;
                        foreach (var (key, value) in equipmentInType)
                        {
                            items[key] = new HakushItemModel
                            {
                                ChineseName = value.ChineseSimplified.Name,
                                EnglishName = value.English.Name,
                                ItemPropertyType = -1,
                                RankType = -1,
                                ItemType = url.Key,
                                Icon = ProcessIconUrl(value.Icon, url.Key),
                                IsCompleted = true,
                                EquipmentDescription = value.ChineseSimplified.Description,
                                EquipmentDetailedDescription = value.ChineseSimplified.DetailedDescription
                            };
                        }
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