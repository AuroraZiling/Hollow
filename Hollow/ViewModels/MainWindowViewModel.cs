using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Services.MetadataService;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;
using Hollow.Views.Controls;

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
        
        // Metadata
        var metadataProgress = new Progress<Response<string>>(value =>
        {
            if (value.IsSuccess)
            {
                HollowHost.ShowToast(Lang.Toast_MetadataUpdated_Title, $"{value.Message} {Lang.Toast_MetadataUpdated_Message}", NotificationType.Success);
            }
            else
            {
                HollowHost.ShowToast(Lang.Toast_MetadataFailed_Title, $"{value.Message} {Lang.Toast_MetadataFailed_Message}", NotificationType.Error);
            }
        });
        await _metadataService.LoadMetadata(metadataProgress);
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