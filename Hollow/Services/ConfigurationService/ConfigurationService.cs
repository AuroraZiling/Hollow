using System.Globalization;
using System.IO;
using System.Text.Json;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Hollow.Models;
using Hollow.Models.Configs;

namespace Hollow.Services.ConfigurationService;

public class ConfigurationService: IConfigurationService
{
    public ConfigurationService()
    {
        AppConfig = LoadConfiguration();
        CurrentLanguage = AppConfig.Language == "Auto" ? CultureInfo.CurrentCulture.Name : AppConfig.Language;
        I18NExtension.Culture = AppConfig.Language != "Auto" ? new CultureInfo(AppConfig.Language) : CultureInfo.CurrentCulture;
    }

    public AppConfig AppConfig { get; set; }
    
    public string CurrentLanguage { get; set; } // auto -> en-US, zh-CN
    
    public static AppConfig LoadConfiguration()
    {
        Directory.CreateDirectory(AppInfo.BasePath);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
        Directory.CreateDirectory(AppInfo.GachaRecordsDir);
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