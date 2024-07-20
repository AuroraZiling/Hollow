using Avalonia.Controls;
using Hollow.Controls.WebView;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    public static NativeWebView AnnouncementWebView { get; set; } = null!;
    
    public Announcements()
    {
        InitializeComponent();
        
        var announcementsViewModel = App.GetService<AnnouncementsViewModel>();
        DataContext = announcementsViewModel;
        AnnouncementWebView = GameAnnouncementWebView;
        AnnouncementWebView.DomContentLoaded += announcementsViewModel.GameAnnouncementWebView_OnDomContentLoaded;
        AnnouncementWebView.NavigationStarted += announcementsViewModel.GameAnnouncementWebView_OnNavigationStarting;
        AnnouncementWebView.Initialized += announcementsViewModel.GameAnnouncementWebView_OnInitialized;
    }
}