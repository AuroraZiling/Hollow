using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class GameSettings : UserControl
{
    public GameSettings(GameSettingsViewModel gameSettingsViewModel)
    {
        InitializeComponent();
        DataContext = gameSettingsViewModel;
    }
}