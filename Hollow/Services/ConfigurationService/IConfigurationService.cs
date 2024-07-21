using Hollow.Abstractions.Models.Configs;

namespace Hollow.Services.ConfigurationService;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }
    public string CurrentLanguage { get; set; }
    public void Save();
}