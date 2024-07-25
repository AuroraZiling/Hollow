using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts;

namespace Hollow.Services.MetadataService;

public class MetadataService(HttpClient httpClient): IMetadataService
{
    private readonly Dictionary<string, string> _metadataUrls = new()
    {
        {"character", "https://api.hakush.in/zzz/data/character.json"},
        {"weapon", "https://api.hakush.in/zzz/data/weapon.json"},
        {"bangboo", "https://api.hakush.in/zzz/data/bangboo.json"}
    };
    
    public async Task LoadMetadata(IProgress<Response<string>> progress)
    {
        var metadataDir = AppInfo.MetadataDir;
        foreach (var metadataPair in _metadataUrls)
        {
            var metadataPath = Path.Combine(metadataDir, $"{metadataPair.Key}.json");
            if (!File.Exists(metadataPath))
            {
                Directory.CreateDirectory(metadataDir);
                progress.Report(new Response<string>(await DownloadMetadata(metadataPair.Value, metadataPath), metadataPair.Key));
            }
            else
            {
                var metadataCreationTime = ((DateTimeOffset)new FileInfo(metadataPath).CreationTime).ToUnixTimeSeconds();
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (now - metadataCreationTime >= 1209600)
                {
                    progress.Report(new Response<string>(await DownloadMetadata(metadataPair.Value, metadataPath), metadataPair.Key));
                }
            }
        }
    }
    
    public async Task LoadMetadata(string metadataKey, IProgress<Response<string>> progress)
    {
        if (_metadataUrls.TryGetValue(metadataKey, out var metadataUrl))
        {
            var metadataDir = AppInfo.MetadataDir;
            var metadataPath = Path.Combine(metadataDir, $"{metadataKey}.json");
            if (!File.Exists(metadataPath))
            {
                Directory.CreateDirectory(metadataDir);
                progress.Report(new Response<string>(await DownloadMetadata(metadataUrl, metadataPath), metadataKey));
            }
            else
            {
                var metadataCreationTime = ((DateTimeOffset)new FileInfo(metadataPath).CreationTime).ToUnixTimeSeconds();
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (now - metadataCreationTime >= 1209600)
                {
                    progress.Report(new Response<string>(await DownloadMetadata(metadataUrl, metadataPath), metadataKey));
                }
            }
        }
        
    }
    private async Task<bool> DownloadMetadata(string metadataUrl, string metadataPath)
    {
        try
        {
            var metadata = await httpClient.GetStringAsync(metadataUrl);
            if(File.Exists(metadataPath))
            {
                File.Delete(metadataPath);
            }
            await File.WriteAllTextAsync(metadataPath, metadata);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}