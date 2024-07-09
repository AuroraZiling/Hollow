using Avalonia.Controls;
using Hollow.Controls.WebView.Avalonia.WebView;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    public static WebView AnnouncementWebView { get; set; } = null!;
    
    public Announcements()
    {
        InitializeComponent();
        
        var announcementsViewModel = App.GetService<AnnouncementsViewModel>();
        DataContext = announcementsViewModel;
        AnnouncementWebView = GameAnnouncementWebView;
        AnnouncementWebView.NavigationStarting += announcementsViewModel.GameAnnouncementWebView_OnNavigationStarting;
        AnnouncementWebView.Initialized += announcementsViewModel.GameAnnouncementWebView_OnInitialized;
        AnnouncementWebView.WebViewCreated += announcementsViewModel.GameAnnouncementWebView_OnWebViewCreated;
    }
}