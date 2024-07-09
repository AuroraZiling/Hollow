using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Enums;
using Hollow.Services.ConfigurationService;

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