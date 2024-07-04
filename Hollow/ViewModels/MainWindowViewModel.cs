using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Core.MiHoYoLauncher.Models;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserControl _currentView;

    [ObservableProperty] private bool _displayHome = true;
    [ObservableProperty] private bool _displayGameSettings;
    [ObservableProperty] private bool _displaySignalSearch;
    [ObservableProperty] private bool _displayScreenshots;
    [ObservableProperty] private bool _displaySettings;
    
    [ObservableProperty] private string? _backgroundImageEffect = "";
    [ObservableProperty] private double _dragAreaOpacity;

    [ObservableProperty] private Bitmap _gameIcon;
    [ObservableProperty] private string? _backgroundUrl = "avares://Hollow/Assets/default_bg.webp";
    
    private readonly INavigationService _navigationService;
    private readonly IMiHoYoLauncherService _miHoYoLauncherService;

    public MainWindowViewModel(INavigationService navigationService, IMiHoYoLauncherService miHoYoLauncherService)
    {
        _navigationService = navigationService;
        _miHoYoLauncherService = miHoYoLauncherService;

        _currentView = _navigationService.CurrentView;
        _navigationService.CurrentViewChanged += OnNavigating;
        
        // Load game icon
        GameIcon = new Bitmap(AssetLoader.Open(new Uri("avares://Hollow/Assets/ZZZ_Logo.jpg")));
        using var memory = new MemoryStream();
        GameIcon.Save(memory, 100);
        memory.Position = 0;
        GameIcon = Bitmap.DecodeToHeight(memory, 100, BitmapInterpolationMode.HighQuality);

        _ = LoadOnlineResources();
    }

    private async Task LoadOnlineResources()
    {
        // Background
        var allGameBasicInfo = await _miHoYoLauncherService.GetAllGameBasicInfo();
        BackgroundUrl = allGameBasicInfo?.Data.GameInfo[0].Backgrounds[0].Image.Url ?? BackgroundUrl;
    }
    
    private void OnNavigating()
    {
        CurrentView = _navigationService.CurrentView;
        DisplayHome = _navigationService.CurrentViewName == "Home";
        DisplayGameSettings = _navigationService.CurrentViewName == "GameSettings";
        DisplaySignalSearch = _navigationService.CurrentViewName == "SignalSearch";
        DisplayScreenshots = _navigationService.CurrentViewName == "Screenshots";
        DisplaySettings = _navigationService.CurrentViewName == "Settings";
        
        DragAreaOpacity = DisplayHome ? 0 : 1;
        BackgroundImageEffect = DisplayHome ? "": "blur(40)";
    }
    
    [RelayCommand]
    private void Navigate(string destination)
    {
        _navigationService.Navigate(destination);
        OnNavigating();
    }
}