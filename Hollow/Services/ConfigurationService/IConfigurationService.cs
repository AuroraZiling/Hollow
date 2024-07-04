using Hollow.Models.Configs;

namespace Hollow.Services.ConfigurationService;

public interface IConfigurationService
{
    public AppConfig? AppConfig { get; set; }

    public void Save();
}