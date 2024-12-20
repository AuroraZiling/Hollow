﻿using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Services;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Views.Controls;
using Serilog;

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
    private readonly IMetadataService _metadataService;
    
    public static Action? NavigatedToHome { get; set; }

    public MainWindowViewModel(INavigationService navigationService, IMiHoYoLauncherService miHoYoLauncherService, IMetadataService metadataService)
    {
        _navigationService = navigationService;
        _miHoYoLauncherService = miHoYoLauncherService;
        _metadataService = metadataService;

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
        Log.Information("[MainWindow] Background loaded");
        
        // Metadata
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
        await _metadataService.LoadItemMetadata(metadataProgress); 
    }
    
    private void OnNavigating()
    {
        Blur = DisplayPageId == 0 ? 1 : 20;
        CoverageOpacity = DisplayPageId == 0 ? 0 : 1;
        NavigationOpacity = DisplayPageId == 0 ? 1 : 0;

        DisplayPageId = _navigationService.CurrentViewId;
        
        if (DisplayPageId == 0)
            NavigatedToHome?.Invoke();
    }

    [RelayCommand]
    private void ChangePage(int destinationId)
    {
        _navigationService.Navigate(destinationId);
    }
}