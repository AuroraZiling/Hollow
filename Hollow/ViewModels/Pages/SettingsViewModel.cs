using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Services;
using Hollow.Extensions;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Views.Controls;
using Hollow.Views.Pages;
using Serilog;

namespace Hollow.ViewModels.Pages;

public partial class SettingsViewModel : ViewModelBase, IViewModelBase
{
    [ObservableProperty] private string _appVersion = AppInfo.AppVersion;
    
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;
    private readonly IGameService _gameService;
    private readonly IMetadataService _metadataService;

    public void Navigated()
    {
        if (_navigationService.CurrentViewName == nameof(Settings))
        {
            CacheSize = PlatformHelper.GetCacheFolderMegabytes();
        }
    }
    public SettingsViewModel(IConfigurationService configurationService, INavigationService navigationService, IGameService gameService, IMetadataService metadataService)
    {
        _configurationService = configurationService;
        _navigationService = navigationService;
        _gameService = gameService;
        _metadataService = metadataService;

        //navigationService.CurrentViewChanged += Navigated;

        // Game Init
        GameDirectory = _configurationService.AppConfig.Game.Directory;
        GameArguments = _configurationService.AppConfig.Game.Arguments;
        
        // Records Init
        FullUpdate = _configurationService.AppConfig.Records.FullUpdate;
        
        // Language Init
        foreach (var languagePair in GetLanguage.LanguageList)
        {
            Languages.Add(languagePair.Key);
        }
        var language = _configurationService.AppConfig.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
        
        // Game Init
        CheckGameDirectory(_configurationService.AppConfig.Game.Directory, true);
        
        _navigationService.CurrentViewChanged += Navigated;
        Log.Information("[Settings] Configuration loaded");
    }

    #region Game

    [ObservableProperty] private string _gameDirectory;
    [ObservableProperty] private string _gameBiz = Lang.Service_Game_Unknown;
    [ObservableProperty] private string _gameArguments;
    
    [RelayCommand]
    private async Task BrowseGameDirectory()
    {
        var directory = await PlatformHelper.OpenFolderPickerForPath();
        CheckGameDirectory(directory);
    }

    private void CheckGameDirectory(string directory, bool removeIfInvalid = false)
    {
        if (_gameService.ValidateGameDirectory(directory))
        {
            GameDirectory = directory;
            GameBiz = _gameService.GameBiz.ToI18NString();
            _configurationService.AppConfig.Game.Directory = GameDirectory;
            _configurationService.Save();
            Log.Information("[Settings] Game directory changed to {GameDirectory}", GameDirectory);
        }
        else if (!string.IsNullOrWhiteSpace(directory))
        {
            HollowHost.ShowToast(Lang.Toast_InvalidGameDirectory_Title, Lang.Toast_InvalidGameDirectory_Message, NotificationType.Error);
            if (removeIfInvalid)
            {
                GameDirectory = string.Empty;
                _configurationService.AppConfig.Game.Directory = GameDirectory;
                _configurationService.Save();
            }
            Log.Warning("[Settings] Invalid game directory: {GameDirectory}", directory);
        }
    }
    
    [RelayCommand]
    private async Task OpenGameDirectory()
    {
        if (!string.IsNullOrWhiteSpace(GameDirectory))
        {
            await App.MainWindowInstance.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(GameDirectory));
        }
    }

    [RelayCommand]
    private void SaveGameArguments()
    {
        _configurationService.AppConfig.Game.Arguments = GameArguments;
        _configurationService.Save();
    }

    #endregion

    #region Records

    [ObservableProperty] private bool _fullUpdate;
    
    [RelayCommand]
    private void ChangeFullUpdate()
    {
        _configurationService.AppConfig.Records.FullUpdate = FullUpdate;
        _configurationService.Save();
    }

    #endregion

    #region Language

    [ObservableProperty] private ObservableCollection<string> _languages = ["Auto"];
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        var language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        I18NExtension.Culture = language != "Auto" ? new CultureInfo(language) : CultureInfo.CurrentCulture;
        _configurationService.AppConfig.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.CurrentLanguage = language != "Auto" ? language : CultureInfo.CurrentCulture.Name;
        _configurationService.Save();
        GameBiz = _gameService.GameBiz.ToI18NString();
        Log.Information("[Settings] Language changed to {Language}", Language);
    }

    #endregion

    #region Metadata

    [RelayCommand]
    private async Task CheckMetadata()
    {
        var metadataProgress = new Progress<Response<string>>(value =>
        {
            if (value.IsSuccess)
            {
                HollowHost.ShowToast(Lang.Toast_MetadataUpdated_Title, "", NotificationType.Success);
            }
            else
            {
                HollowHost.ShowToast(Lang.Toast_MetadataFailed_Title, "", NotificationType.Error);
            }
        });
        await _metadataService.LoadItemMetadata(metadataProgress, true);
    }

    #endregion

    #region Storage

    [RelayCommand]
    private async Task OpenAppDirectory()
    {
        await App.MainWindowInstance.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(Environment.CurrentDirectory));
    }
    
    [ObservableProperty] private double _cacheSize;
    
    [RelayCommand]
    private void ClearCache()
    {
        PlatformHelper.ClearCacheFolder();
        HollowHost.ShowToast(Lang.Toast_CacheCleared_Title, "", NotificationType.Success);
        CacheSize = PlatformHelper.GetCacheFolderMegabytes();
    }

    #endregion
}