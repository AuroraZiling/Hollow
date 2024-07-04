using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Core.MiHoYoLauncher.Models;
using Hollow.Helpers;
using Hollow.Models.Pages.Home;
using Hollow.Services.MiHoYoLauncherService;

namespace Hollow.ViewModels.Pages;

public partial class HomeViewModel: ViewModelBase
{
    [ObservableProperty] private ObservableCollection<BannerModel> _banners = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _activities = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _notices = [];
    [ObservableProperty] private ObservableCollection<ZzzGameContentDataContentPost> _info = [];
    
    private readonly IMiHoYoLauncherService _miHoYoLauncherService;
    private readonly HttpClient _httpClient;

    public HomeViewModel(IMiHoYoLauncherService miHoYoLauncherService, HttpClient httpClient)
    {
        _miHoYoLauncherService = miHoYoLauncherService;
        _httpClient = httpClient;
        _ = LoadContents();
    }
    
    [ObservableProperty] private bool _isGameRunning;

    [RelayCommand]
    private async Task StartGame()
    {
        IsGameRunning = true;
        await Task.Delay(1000);
        IsGameRunning = false;
    }

    private async Task LoadContents()
    {
        var gameContent = await _miHoYoLauncherService.GetGameContent();

        // Banners
        var banners = gameContent?.Data.Content.Banners;
        if (banners is null)
            return;
        foreach (var banner in banners)
        {
            using var stream = await _httpClient.GetAsync(banner.Image.Url);
            Banners.Add(new BannerModel {Link = banner.Image.Link, Image = StreamToBitmap.Convert(await stream.Content.ReadAsStreamAsync(), 320)});
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
}