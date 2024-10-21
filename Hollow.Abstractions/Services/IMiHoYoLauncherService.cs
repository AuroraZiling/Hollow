using Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;

namespace Hollow.Abstractions.Services;

public interface IMiHoYoLauncherService
{
    public Task<ZzzGameInfo?> GetGameInfo();
    public Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo();
    public Task<ZzzGameContent?> GetGameContent();
}