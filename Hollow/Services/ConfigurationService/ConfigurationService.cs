using System.IO;
using System.Text.Json;
using Hollow.Models;
using Hollow.Models.Configs;

namespace Hollow.Services.ConfigurationService;

public class ConfigurationService: IConfigurationService
{
    public AppConfig AppConfig { get; set; } = LoadConfiguration();
    
    public static AppConfig LoadConfiguration()
    {
        Directory.CreateDirectory(AppInfo.CrushesDir);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
        if (!File.Exists(AppInfo.ConfigPath))
        {
            File.WriteAllText(AppInfo.ConfigPath, JsonSerializer.Serialize(new AppConfig(), new JsonSerializerOptions { WriteIndented = true }));
        }
        return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(AppInfo.ConfigPath))!;
    }

    public void Save()
    {
        File.WriteAllText(AppInfo.ConfigPath, JsonSerializer.Serialize(AppConfig, new JsonSerializerOptions { WriteIndented = true }));
    }
}