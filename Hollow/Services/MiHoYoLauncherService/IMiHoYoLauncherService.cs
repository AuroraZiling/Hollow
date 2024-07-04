using System.Threading.Tasks;
using Hollow.Core.MiHoYoLauncher.Models;

namespace Hollow.Services.MiHoYoLauncherService;

public interface IMiHoYoLauncherService
{
    public Task<ZzzGameInfo?> GetGameInfo();
    public Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo();
    public Task<ZzzGameContent?> GetGameContent();

}