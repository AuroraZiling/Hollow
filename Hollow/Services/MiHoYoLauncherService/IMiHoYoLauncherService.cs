using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;

namespace Hollow.Services.MiHoYoLauncherService;

public interface IMiHoYoLauncherService
{
    public Task<ZzzGameInfo?> GetGameInfo();
    public Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo();
    public Task<ZzzGameContent?> GetGameContent();
}