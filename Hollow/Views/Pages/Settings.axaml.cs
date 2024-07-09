using Avalonia.Controls;
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