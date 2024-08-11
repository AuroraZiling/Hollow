using Avalonia.Controls;
using Avalonia.Interactivity;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        DataContext = App.GetService<HomeViewModel>();
    }

    private void PreviousBanner(object? _1, RoutedEventArgs _2)
    {
        Banners.Previous();
    }

    private void NextBanner(object? _1, RoutedEventArgs _2)
    {
        Banners.Next();
    }
}