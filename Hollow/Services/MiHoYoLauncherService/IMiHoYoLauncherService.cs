using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;
using Hollow.Models.Pages.Announcement;

namespace Hollow.Services.MiHoYoLauncherService;

public interface IMiHoYoLauncherService
{
    public Task<ZzzGameInfo?> GetGameInfo();
    public Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo();
    public Task<ZzzGameContent?> GetGameContent();
    public Task<Dictionary<int, List<AnnouncementModel>>?> GetAnnouncement();

}