﻿using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;
using Hollow.Abstractions.Services;
using Hollow.Helpers;
using Hollow.Models.Home;
using Hollow.Views.Pages;
using Serilog;

namespace Hollow.ViewModels.Pages;

public partial class HomeViewModel: ViewModelBase, IViewModelBase
{
    [ObservableProperty] private string _versionNewsImageUrl = "";
    [ObservableProperty] private string _versionNewsUrl = "";

    [ObservableProperty] private ObservableCollection<BannerModel> _banners = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _activities = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _notices = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _info = [];
    
    [ObservableProperty] private double _startGameNoticeOpacity;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsGameStartButtonEnabled))] private bool _startGameReady;
    [ObservableProperty] private string _gameVersion = "Unknown";
    
    private readonly IMiHoYoLauncherService _miHoYoLauncherService;
    private readonly IConfigurationService _configurationService;
    private readonly IGameService _gameService;
    private readonly HttpClient _httpClient;
    private readonly INavigationService _navigationService;

    public HomeViewModel(IMiHoYoLauncherService miHoYoLauncherService, HttpClient httpClient, IConfigurationService configurationService, IGameService gameService, INavigationService navigationService)
    {
        _miHoYoLauncherService = miHoYoLauncherService;
        _httpClient = httpClient;
        _configurationService = configurationService;
        _gameService = gameService;
        _navigationService = navigationService;

        _ = LoadContents();
        CheckStartGameReady();
        
        MainWindowViewModel.NavigatedToHome += Navigated;
    }

    private void CheckStartGameReady()
    {
        StartGameReady = _gameService.ValidateGameDirectory(_configurationService.AppConfig.Game.Directory);
        StartGameNoticeOpacity = !StartGameReady ? 1 : 0;
        if (StartGameReady)
            GameVersion = _gameService.GameVersion;
    }
    
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsGameStartButtonEnabled))] private bool _isGameStarting;
    public bool IsGameStartButtonEnabled => !IsGameStarting && StartGameReady;

    [RelayCommand]
    private void StartGame()
    {
        IsGameStarting = true;
        _gameService.StartGame();
        IsGameStarting = false;
    }
    
    [RelayCommand]
    private void NavigateToSettings()
    {
        App.GetService<INavigationService>().Navigate(5);
    }

    private async Task LoadContents()
    {
        var gameContent = await _miHoYoLauncherService.GetGameContent();
        var allGameBasicInfo = await _miHoYoLauncherService.GetAllGameBasicInfo();

        // Version News
        VersionNewsImageUrl = allGameBasicInfo?.Data.GameInfo[0].Backgrounds[0].Icon.Url ?? "";
        VersionNewsUrl = allGameBasicInfo?.Data.GameInfo[0].Backgrounds[0].Icon.Link ?? "";
        Log.Information("[Home] Version news loaded");

        // Banners
        var banners = gameContent?.Data.Content.Banners;
        if (banners is null)
            return;
        foreach (var banner in banners)
        {
            using var stream = await _httpClient.GetAsync(banner.Image.Url);
            Banners.Add(new BannerModel {Link = banner.Image.Link, Image = BitmapOperations.Convert(await stream.Content.ReadAsStreamAsync(), 320)});
        }
        Log.Information("[Home] Banners loaded");
        
        // Posts
        var posts = gameContent?.Data.Content.Posts;
        if (posts is null)
            return;
        
        foreach (var post in posts)
        {
            switch (post.Type)
            {
                case "POST_TYPE_ACTIVITY":
                    Activities.Add(post);
                    break;
                case "POST_TYPE_ANNOUNCE":
                    Notices.Add(post);
                    break;
                case "POST_TYPE_INFO":
                    Info.Add(post);
                    break;
            }
        }
        Log.Information("[Home] Posts loaded");
    }

    public void Navigated()
    {
        if (_navigationService.CurrentViewName == nameof(Home))
        {
            CheckStartGameReady();
        }
    }
}