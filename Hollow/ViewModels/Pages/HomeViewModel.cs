using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Core.MiHoYoLauncher.Models;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Models.Pages.Home;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GameService;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;

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
    private readonly IGameService _gameService;
    private readonly HttpClient _httpClient;

    public HomeViewModel(IMiHoYoLauncherService miHoYoLauncherService, HttpClient httpClient, IConfigurationService configurationService, IGameService gameService)
    {
        _miHoYoLauncherService = miHoYoLauncherService;
        _httpClient = httpClient;
        _gameService = gameService;

        _ = LoadContents();
        CheckStartGameReady();

        MainWindowViewModel.NavigatedToHome += Navigated;
    }

    private void CheckStartGameReady()
    {
        StartGameReady = _gameService.CheckGameStartReady();
        StartGameNoticeOpacity = !StartGameReady ? 1 : 0;
        if (StartGameReady)
            GameVersion = _gameService.GetGameVersion();
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
        App.GetService<INavigationService>().Navigate("Settings");
    }

    private async Task LoadContents()
    {
        var gameContent = await _miHoYoLauncherService.GetGameContent();
        var allGameBasicInfo = await _miHoYoLauncherService.GetAllGameBasicInfo();

        // Version News
        VersionNewsImageUrl = allGameBasicInfo?.Data.GameInfo[0].Backgrounds[0].Icon.Url ?? "";
        VersionNewsUrl = allGameBasicInfo?.Data.GameInfo[0].Backgrounds[0].Icon.Link ?? "";

        // Banners
        var banners = gameContent?.Data.Content.Banners;
        if (banners is null)
            return;
        foreach (var banner in banners)
        {
            using var stream = await _httpClient.GetAsync(banner.Image.Url);
            Banners.Add(new BannerModel {Link = banner.Image.Link, Image = BitmapOperations.Convert(await stream.Content.ReadAsStreamAsync(), 320)});
        }
        
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
    }

    public void Navigated()
    {
        CheckStartGameReady();
    }
}