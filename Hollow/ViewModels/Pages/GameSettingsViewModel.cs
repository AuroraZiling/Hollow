using System.Globalization;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.Input;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels.Pages;

public partial class GameSettingsViewModel(INavigationService navigationService): ViewModelBase
{
    [RelayCommand]
    private void ChangeLanguage()
    {
        navigationService.Navigate("Home");
    }
}