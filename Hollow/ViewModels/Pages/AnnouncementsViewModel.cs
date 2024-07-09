using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.WebView.Windows.Core;
using AvaloniaWebView;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Helpers;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;
using Hollow.Views.Pages;
using WebViewCore.Events;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel(
    IMiHoYoLauncherService miHoYoLauncherService,
    INavigationService navigationService)
    : ViewModelBase, IViewModelBase
{
    private const string Script = """
                                   window.onload = function() {
                                   let root = document.getElementById("root");
                                   root.style.backgroundColor = "#313131";
                                   root.style.backgroundRepeat = "no-repeat";
                                   root.style.backgroundPosition = "left bottom";
                                   root.style.backgroundSize = "cover";
                                   
                                   let bodyhome = document.getElementsByClassName("home__body home__body--pc");
                                   if (bodyhome.length > 0) {
                                       bodyhome[0].style.transform = "scale(1.43, 1.47)";
                                       bodyhome[0].style.marginTop = "-3px";
                                   }
                                   
                                   let home = document.getElementsByClassName("home");
                                   if (home.length > 0) {
                                       home[0].style.background = "transparent";
                                   }
                                         let homemask = document.getElementsByClassName("home__mask");
                                   if (homemask.length > 0) {
                                       homemask[0].remove();
                                   } 

                                   // Apply the MiSans font
                                   var link = document.createElement('link');
                                   link.type = 'text/css';
                                   link.rel = 'stylesheet';
                                   document.head.appendChild(link);
                                   link.href = 'https://cdn.jsdelivr.net/npm/misans@4.0.0/lib/Normal/MiSans-Medium.min.css';
                                   let innerann = document.getElementsByClassName("inner-ann");
                                   if (innerann.length > 0) {
                                       innerann[0].style.fontFamily = 'MiSans';
                                   } 

                                   // Remove the close button
                                   let homeclose = document.getElementsByClassName("home__close");
                                   if (homeclose.length > 0) {
                                       homeclose[0].remove();
                                   }
                                   };

                                   """;

    private const string AnnouncementUrl =
        "https://sdk.mihoyo.com/nap/announcement/index.html?auth_appid=announcement&authkey_ver=1&bundle_id=nap_cn&channel_id=1&game=nap&game_biz=nap_cn&lang=zh-cn&level=60&platform=pc&region=prod_gf_cn&sdk_presentation_style=fullscreen&sdk_screen_transparent=true&sign_type=2&uid=100000000&version=2.27#/";

    public void Navigated()
    {
        
    }

    [ObservableProperty] private string? _announcementHtmlContent;
    [ObservableProperty] private double _announcementLoadingCoverageOpacity = 1;
    [ObservableProperty] private bool _announcementLoaded;

    public void GameAnnouncementWebView_OnWebViewCreated(object? sender, WebViewCreatedEventArgs e)
    {
        AnnouncementLoadingCoverageOpacity = 1;
        AnnouncementLoaded = false;
        //TODO: Platform specific
        if (Announcements.AnnouncementWebView.PlatformWebView is WebView2Core webView2Core)
        {
            webView2Core.CoreWebView2!.DOMContentLoaded += async (_, _) =>
            {
                await webView2Core.CoreWebView2.ExecuteScriptAsync(Script);
                await Task.Delay(1500);
                AnnouncementLoadingCoverageOpacity = 0;
                AnnouncementLoaded = true;
            };
        }
    }

    public void GameAnnouncementWebView_OnInitialized(object? sender, EventArgs e)
    {
        Announcements.AnnouncementWebView.Url = new Uri(AnnouncementUrl);
    }

    public void GameAnnouncementWebView_OnNavigationStarting(object? sender, WebViewUrlLoadingEventArg e)
    {
        if (e.Url!.ToString().StartsWith("uniwebview://open_url?url="))
        {
            HtmlHelper.OpenUrl(e.Url!.ToString().Replace("uniwebview://open_url?url=", ""));
        }
    }
}