using System.Globalization;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Enums;
using Hollow.Services.ConfigurationService;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels.Pages;

public partial class GameSettingsViewModel(INavigationService navigationService, IConfigurationService configurationService): ViewModelBase, IViewModelBase
{
    [RelayCommand]
    private void ChangeLanguage()
    {
        HollowHost.ShowToast("info", configurationService.CurrentLanguage, NotificationType.Info);
    }

    public void Navigated()
    {
        
    }
}