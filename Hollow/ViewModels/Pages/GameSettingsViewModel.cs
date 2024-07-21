using CommunityToolkit.Mvvm.Input;
using Hollow.Enums;
using Hollow.Services.ConfigurationService;
using Hollow.Views.Controls;

namespace Hollow.ViewModels.Pages;

public partial class GameSettingsViewModel(IConfigurationService configurationService): ViewModelBase, IViewModelBase
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