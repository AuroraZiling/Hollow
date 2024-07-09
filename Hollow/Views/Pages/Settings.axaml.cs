using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
        DataContext = App.GetService<SettingsViewModel>();
    }
}