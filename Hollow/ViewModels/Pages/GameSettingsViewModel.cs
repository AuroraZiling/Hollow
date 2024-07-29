using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models;
using Hollow.Enums;
using Hollow.Languages;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Hollow.Services.NavigationService;
using Hollow.Views.Controls;
using Hollow.Views.Pages;

namespace Hollow.ViewModels.Pages;

public partial class GameSettingsViewModel : ViewModelBase, IViewModelBase
{
    [ObservableProperty]
    private bool _isSettingsChanged;
    
    [ObservableProperty]
    private bool _isGameDirectoryValid;

    
    [RelayCommand]
    private void SettingsChanged() => IsSettingsChanged = true;

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    private void SaveSettings()
    {
        if (ValidateSettings())
        {
            Helpers.Registry.Screen.SaveScreenSettings(_gameService.GameBiz, int.Parse(ScreenResolutionWidth), int.Parse(ScreenResolutionHeight), IsFullScreen);
            IsSettingsChanged = false;
            HollowHost.ShowToast(Lang.Toast_Common_Saved_Title, "", NotificationType.Success);
        }
    }
    
    private bool ValidateSettings()
    {
        var screenResponse = ValidateScreenRegistry();
        if (!screenResponse.IsSuccess)
        {
            HollowHost.ShowToast(Lang.Toast_Common_Error_Title, screenResponse.Message, NotificationType.Error);
            return false;
        }

        return true;
    }
    
    #region Screen

    [ObservableProperty]
    private string _screenResolutionWidth = "";
    partial void OnScreenResolutionWidthChanged(string value) => SettingsChanged();
    
    [ObservableProperty]
    private string _screenResolutionHeight = "";
    partial void OnScreenResolutionHeightChanged(string value) => SettingsChanged();

    [ObservableProperty]
    private bool _isFullScreen;
    partial void OnIsFullScreenChanged(bool value) => SettingsChanged();


    private readonly IGameService _gameService;
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;

    public GameSettingsViewModel(IGameService gameService, IConfigurationService configurationService, INavigationService navigationService)
    {
        _gameService = gameService;
        _configurationService = configurationService;
        _navigationService = navigationService;
        
        navigationService.CurrentViewChanged += Navigated;
    }

    [SupportedOSPlatform("Windows")]
    private void LoadScreenRegistry()
    {
        var screen = new Helpers.Registry.Screen(_gameService.GameBiz);
        ScreenResolutionWidth = screen.ResolutionWidth.ToString();
        ScreenResolutionHeight = screen.ResolutionHeight.ToString();
        IsFullScreen = screen.IsFullScreen;
    }
    
    private Response<string> ValidateScreenRegistry()
    {
        if (int.TryParse(ScreenResolutionWidth, out var width) && int.TryParse(ScreenResolutionHeight, out var height))
        {
            if (width < 640 || height < 480)
            {
                return new Response<string>(false, Lang.Toast_GameSettingsScreen_ResolutionTooSmall_Message);
            }
            if (width > 7680 || height > 4320)
            {
                return new Response<string>(false, Lang.Toast_GameSettingsScreen_ResolutionTooLarge_Message);
            }
            return new Response<string>(true);
        }

        return new Response<string>(false, Lang.Toast_GameSettingsScreen_ResolutionNotNumber_Message);
    }

    #endregion
    

    public void Navigated()
    {
        if (_navigationService.CurrentViewName == nameof(GameSettings) && _gameService.ValidateGameDirectory(_configurationService.AppConfig.Game.Directory))
        {
            IsGameDirectoryValid = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LoadScreenRegistry();
            }
            IsSettingsChanged = false;
        }
        else
        {
            IsGameDirectoryValid = false;
        }
    }
}