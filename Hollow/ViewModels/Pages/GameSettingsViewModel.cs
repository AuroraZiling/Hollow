using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Hollow.Services.NavigationService;
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
    private void SaveSettings() => IsSettingsChanged = false;
    
    #region Screen

    [ObservableProperty]
    private string _screenResolutionWidth = "";
    [ObservableProperty]
    private string _screenResolutionHeight = "";
    [ObservableProperty]
    private bool _isFullScreen;

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

    private void LoadScreenRegistry()
    {
        
    }

    #endregion
    

    public void Navigated()
    {
        if (_navigationService.CurrentViewName == nameof(GameSettings) && _gameService.ValidateGameDirectory(_configurationService.AppConfig.Game.Directory))
        {
            IsGameDirectoryValid = true;
            LoadScreenRegistry();
            IsSettingsChanged = false;
        }
        else
        {
            IsGameDirectoryValid = false;
        }
    }
}