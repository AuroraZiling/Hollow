using Avalonia.Controls;
using Hollow.ViewModels.Pages;
using WebView = Hollow.Controls.WebView.WebView;

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