using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Helpers;
using Serilog;

namespace Hollow.Services.MetadataService;

public class MetadataService(HttpClient httpClient): IMetadataService
{
    public async Task LoadItemMetadata(IProgress<Response<string>> progress)
    {
        var urls = new List<string> {
            "https://api.hakush.in/zzz/data/character.json", "https://api.hakush.in/zzz/data/weapon.json",
            "https://api.hakush.in/zzz/data/bangboo.json"
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
            if (now - metadataCreationTime >= 1209600)
            {
                progress.Report(new Response<string>(await DownloadItemMetadata()));
            }
        }
        
        async Task<bool> DownloadItemMetadata()
        {
            try
            {
                var items = new Dictionary<string, ItemModel>();
                foreach (var url in urls)
                {
                    var metadata = await httpClient.GetStringAsync(url);
                    var itemsInType = JsonSerializer.Deserialize<Dictionary<string, ItemModel>>(metadata, HollowJsonSerializer.Options)!;
                    
                    foreach (var (key, value) in itemsInType)
                    {
                        items[key] = value;
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