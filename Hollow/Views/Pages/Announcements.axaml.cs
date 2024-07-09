using Avalonia.Controls;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    public static Controls.WebView.Avalonia.WebView.WebView AnnouncementWebView { get; set; } = null!;
    
    private readonly IMiHoYoLauncherService _miHoYoLauncherService;
    public Announcements(AnnouncementsViewModel announcementsViewModel, IMiHoYoLauncherService miHoYoLauncherService)
    {
        _miHoYoLauncherService = miHoYoLauncherService;
        InitializeComponent();
        DataContext = announcementsViewModel;
        AnnouncementWebView = GameAnnouncementWebView;
        AnnouncementWebView.NavigationStarting += announcementsViewModel.GameAnnouncementWebView_OnNavigationStarting;
        AnnouncementWebView.Initialized += announcementsViewModel.GameAnnouncementWebView_OnInitialized;
        AnnouncementWebView.WebViewCreated += announcementsViewModel.GameAnnouncementWebView_OnWebViewCreated;
    }
}