using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Models.Pages.Announcement;
using Hollow.Services.ConfigurationService;
using Hollow.Services.MiHoYoLauncherService;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
    }

    private readonly IMiHoYoLauncherService _miHoYoLauncherService;

    [ObservableProperty] private List<AnnouncementModel> _gameAnnouncements = [];
    [ObservableProperty] private List<AnnouncementModel> _activityAnnouncements = [];

    public AnnouncementsViewModel(IMiHoYoLauncherService miHoYoLauncherService)
    {
        _miHoYoLauncherService = miHoYoLauncherService;

        _ = LoadAnnouncements();
    }

    private async Task LoadAnnouncements()
    {
        var announcements = await _miHoYoLauncherService.GetAnnouncement();
        if (announcements is null)
        {
            return;
        }
        GameAnnouncements = announcements[3];
        ActivityAnnouncements = announcements[4];
    }
}