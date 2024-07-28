using System.Globalization;
using System.IO;
using System.Text.Json;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.Configs;
using Hollow.Helpers;
using Serilog;

namespace Hollow.Services.ConfigurationService;

public class ConfigurationService: IConfigurationService
{
    public ConfigurationService()
    {
        AppConfig = LoadConfiguration();
        CurrentLanguage = AppConfig.Language == "Auto" ? CultureInfo.CurrentCulture.Name : AppConfig.Language;
        I18NExtension.Culture = AppConfig.Language != "Auto" ? new CultureInfo(AppConfig.Language) : CultureInfo.CurrentCulture;
        
        Log.Information("[ConfigurationService] Initialized (Current Language: {Language})", CurrentLanguage);
    }

    public AppConfig AppConfig { get; set; }
    
    public string CurrentLanguage { get; set; } // auto -> en-US, zh-CN
    
    public static AppConfig LoadConfiguration()
    {
        Directory.CreateDirectory(AppInfo.BasePath);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
        Directory.CreateDirectory(AppInfo.MetadataDir);
        if (!File.Exists(AppInfo.ConfigPath))
        {
            File.WriteAllText(AppInfo.ConfigPath, JsonSerializer.Serialize(new AppConfig(), HollowJsonSerializer.Options));
        }
        Log.Information("[ConfigurationService] Configuration loaded");
        return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(AppInfo.ConfigPath))!;
    }

    public void Save()
    {
        File.WriteAllText(AppInfo.ConfigPath, JsonSerializer.Serialize(AppConfig, HollowJsonSerializer.Options));
    }
}