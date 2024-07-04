using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    public Announcements(AnnouncementsViewModel announcementsViewModel)
    {
        InitializeComponent();
        DataContext = announcementsViewModel;
    }
}