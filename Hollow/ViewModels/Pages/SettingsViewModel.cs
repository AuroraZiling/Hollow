using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using Hollow.Languages;
using Hollow.Services.ConfigurationService;

namespace Hollow.ViewModels.Pages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IConfigurationService _configurationService;
    public SettingsViewModel(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        
        // Language Init
        foreach (var languagePair in GetLanguage.LanguageList)
        {
            Languages.Add(languagePair.Key);
        }
        var language = _configurationService.AppConfig.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
    }
    
    [ObservableProperty] private ObservableCollection<string> _languages = ["Auto"];
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        var language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        I18NExtension.Culture = language != "Auto" ? new CultureInfo(language) : CultureInfo.CurrentCulture;
        _configurationService.AppConfig.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.Save();
    }
}