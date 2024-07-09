using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        DataContext = App.GetService<HomeViewModel>();
    }

    private void PreviousBanner(object? sender, RoutedEventArgs e)
    {
        Banners.Previous();
    }

    private void NextBanner(object? sender, RoutedEventArgs e)
    {
        Banners.Next();
    }
}