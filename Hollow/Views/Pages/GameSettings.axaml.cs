using Avalonia.Controls;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class GameSettings : UserControl
{
    public GameSettings()
    {
        InitializeComponent();
        DataContext = App.GetService<GameSettingsViewModel>();
    }
}