using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Models.Pages.Announcement;
using Hollow.Services.MiHoYoLauncherService;
using static Hollow.Views.Pages.Announcements;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
    }

    private readonly IMiHoYoLauncherService _miHoYoLauncherService;

    [ObservableProperty] private ObservableCollection<AnnouncementModel> _gameAnnouncements = [];
    [ObservableProperty] private AnnouncementModel? _selectedGameAnnouncement;
    
    [ObservableProperty] private ObservableCollection<AnnouncementModel> _activityAnnouncements = [];
    [ObservableProperty] private AnnouncementModel? _selectedActivityAnnouncement;
    
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
        GameAnnouncements = new ObservableCollection<AnnouncementModel>(announcements[3]);
        SelectedGameAnnouncement = GameAnnouncements[0];
        ActivityAnnouncements = new ObservableCollection<AnnouncementModel>(announcements[4]);
        SelectedActivityAnnouncement = ActivityAnnouncements[0];
    }
    
    [RelayCommand]
    private void ChangeGameAnnouncement()
    {
        GlobalGameAnnouncementWebView?.LoadHtml(SelectedGameAnnouncement!.Content);
    }
}