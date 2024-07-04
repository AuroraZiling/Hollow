using System.Globalization;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Enums;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels.Pages;

public partial class GameSettingsViewModel(INavigationService navigationService): ViewModelBase, IViewModelBase
{
    [RelayCommand]
    private void ChangeLanguage()
    {
        HollowHost.ShowToast("info", "test", NotificationType.Info);
        HollowHost.ShowToast("info", "test", NotificationType.Success);
        HollowHost.ShowToast("info", "test", NotificationType.Warning);
        HollowHost.ShowToast("info", "test", NotificationType.Error);
    }

    public void Navigated()
    {
        
    }
}