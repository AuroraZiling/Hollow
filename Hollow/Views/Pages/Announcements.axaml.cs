using Avalonia.Controls;
using Hollow.ViewModels.Pages;
using WebViewControl;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    public Announcements(AnnouncementsViewModel announcementsViewModel)
    {
        InitializeComponent();
        DataContext = announcementsViewModel;

        GlobalGameAnnouncementWebView = GameAnnouncementWebView;
    }

    public static WebView? GlobalGameAnnouncementWebView { get; set; }
}