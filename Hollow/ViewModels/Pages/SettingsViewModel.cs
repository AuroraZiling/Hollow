using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Hollow.Services.NavigationService;
using Hollow.Views.Controls;
using Hollow.Views.Pages;
using NotificationType = Hollow.Enums.NotificationType;

namespace Hollow.ViewModels.Pages;

public partial class SettingsViewModel : ViewModelBase, IViewModelBase
{
    [ObservableProperty] private string _appVersion = AppInfo.AppVersion;
    
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;

    public void Navigated()
    {
        if (_navigationService.CurrentViewName == nameof(Settings))
        {
            Console.WriteLine(1);
        }
    }
    public SettingsViewModel(IConfigurationService configurationService, INavigationService navigationService)
    {
        _configurationService = configurationService;
        _navigationService = navigationService;
        
        //navigationService.CurrentViewChanged += Navigated;

        // Game Init
        GameDirectory = _configurationService.AppConfig.Game.Directory;
        GameArguments = _configurationService.AppConfig.Game.Arguments;
        
        // Language Init
        foreach (var languagePair in GetLanguage.LanguageList)
        {
            Languages.Add(languagePair.Key);
        }
        var language = _configurationService.AppConfig.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
    }

    #region Game

    [ObservableProperty] private string _gameDirectory;
    [ObservableProperty] private string _gameArguments;
    
    [RelayCommand]
    private async Task BrowseGameDirectory()
    {
        var directory = await StorageHelper.OpenFolderPickerForPath();
        if (GameService.ValidateGameDirectory(directory))
        {
            GameDirectory = directory;
            _configurationService.AppConfig.Game.Directory = GameDirectory;
            _configurationService.Save();
        }
        else if (!string.IsNullOrWhiteSpace(directory))
        {
            await HollowHost.ShowToast(Lang.Toast_InvalidGameDirectory_Title, Lang.Toast_InvalidGameDirectory_Message, NotificationType.Error);
        }
    }
    
    [RelayCommand]
    private void OpenGameDirectory()
    {
        if (!string.IsNullOrWhiteSpace(GameDirectory) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StorageHelper.OpenFolderInExplorer(GameDirectory);
        }
    }

    [RelayCommand]
    private void SaveGameArguments()
    {
        _configurationService.AppConfig.Game.Arguments = GameArguments;
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
    }

    #endregion

    #region Storage

    [RelayCommand]
    private void OpenAppDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StorageHelper.OpenFolderInExplorer(Environment.CurrentDirectory);
        }
    }

    #endregion
}