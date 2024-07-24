using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Models.Pages.Screenshots;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels.Pages;

public partial class ScreenshotsViewModel: ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
        if (_navigationService.CurrentViewName != nameof(Views.Pages.Screenshots)) return;
        if (ValidateScreenshots())
        {
            LoadScreenshots();
        }
    }

    [ObservableProperty] private string _coverageTitle = "";
    [ObservableProperty] private string _coverageMessage = "";

    [ObservableProperty] private bool _gameReady;
    [ObservableProperty] private ScreenshotImageModel _selectedScreenshots;
    [ObservableProperty] private ObservableCollection<ScreenshotImageModel> _screenshots = [];
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;

    public ScreenshotsViewModel(IConfigurationService configurationService, INavigationService navigationService)
    {
        _configurationService = configurationService;
        _navigationService = navigationService;

        _navigationService.CurrentViewChanged += Navigated;

        if (ValidateScreenshots())
        {
            LoadScreenshots();
        }
    }

    private bool ValidateScreenshots()
    {
        // Validation
        GameReady = GameService.ValidateGameDirectory(_configurationService.AppConfig.Game.Directory);
        if (GameReady)
        {
            var screenshotsDirectory = Path.Combine(_configurationService.AppConfig.Game.Directory, "Screenshot");
            if (Directory.Exists(screenshotsDirectory))
            {
                return true;
            }

            CoverageTitle = "Error";
            CoverageMessage = "Screenshot folder not found";
        }
        else
        {
            CoverageTitle = "Error";
            CoverageMessage = "Game not found";
        }

        return false;
    }

    private void LoadScreenshots()
    {
        Screenshots.Clear();
        var screenshotsDirectory = Path.Combine(_configurationService.AppConfig.Game.Directory, "Screenshot");
        var screenshotsFileInfos = new DirectoryInfo(screenshotsDirectory).GetFiles();
        foreach (var screenshotFileInfo in screenshotsFileInfos)
        {
            if (screenshotFileInfo.Extension == ".jpg")
            {
                Screenshots.Add(new ScreenshotImageModel
                {
                    Filename = screenshotFileInfo.Name,
                    FilePath = screenshotFileInfo.FullName,
                    FileCreatedTime = screenshotFileInfo.CreationTime.ToString(CultureInfo.CurrentCulture)
                });
            }
        }
    }
}