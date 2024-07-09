using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using Hollow.Core.MiHoYoLauncher.Models;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private int _displayPageId;
    
    [ObservableProperty] private int _blur;
    [ObservableProperty] private double _coverageOpacity;
    [ObservableProperty] private double _navigationOpacity = 1;

    [ObservableProperty] private Bitmap _gameIcon;
    [ObservableProperty] private string? _backgroundUrl = "avares://Hollow/Assets/default_bg.webp";
    
    private readonly INavigationService _navigationService;
    private readonly IMiHoYoLauncherService _miHoYoLauncherService;
    
    public static Action? NavigatedToHome { get; set; }

    public MainWindowViewModel(INavigationService navigationService, IMiHoYoLauncherService miHoYoLauncherService)
    {
        _navigationService = navigationService;
        _miHoYoLauncherService = miHoYoLauncherService;

        _navigationService.CurrentViewChanged += OnNavigating;
        
        // Load game icon
        GameIcon = BitmapOperations.Decode(new Bitmap(AssetLoader.Open(new Uri("avares://Hollow/Assets/ZZZ_Logo.jpg"))),
            100);

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
        Blur = DisplayPageId == 0 ? 1 : 20;
        CoverageOpacity = DisplayPageId == 0 ? 0 : 1;
        NavigationOpacity = DisplayPageId == 0 ? 1 : 0;
        
        if (DisplayPageId == 0)
            NavigatedToHome?.Invoke();
    }

    [RelayCommand]
    private void ChangePage(int destinationId)
    {
        DisplayPageId = destinationId;
        _navigationService.Navigate(destinationId);
    }
}