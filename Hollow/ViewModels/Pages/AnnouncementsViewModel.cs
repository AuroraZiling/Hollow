using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Helpers;
using Hollow.Views.Controls.WebView;
using Hollow.Views.Pages;
using Serilog;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel
    : ViewModelBase, IViewModelBase
{
    private const string Script =
        """
        window.onload = function() {
            const root = document.getElementById("root");
            root.style.backgroundRepeat = "no-repeat";
            root.style.backgroundPosition = "left bottom";
            root.style.backgroundSize = "cover";
    
            const bodyHome = document.getElementsByClassName("home__body home__body--pc");
            if (bodyHome.length > 0) {
                bodyHome[0].style.transform = "scale(1.33, 1.37)";
            }
    
            const home = document.getElementsByClassName("home");
            if (home.length > 0) {
                home[0].style.background = "transparent";
            }
            
            const homeMask = document.getElementsByClassName("home__mask");
            if (homeMask.length > 0) {
                homeMask[0].remove();
            } 
    
            // Apply the MiSans font
            const link = document.createElement('link');
            link.type = 'text/css';
            link.rel = 'stylesheet';
            document.head.appendChild(link);
            link.href = 'https://cdn.jsdelivr.net/npm/misans@4.0.0/lib/Normal/MiSans-Medium.min.css';
            const innerAnn = document.getElementsByClassName("inner-ann");
            if (innerAnn.length > 0) {
                innerAnn[0].style.fontFamily = 'MiSans';
            } 
    
            // Remove the close button
            const homeClose = document.getElementsByClassName("home__close");
            if (homeClose.length > 0) {
                homeClose[0].remove();
            }
    
            // 重写所有 uniwebview URL 方案
            function rewriteUniWebViewUrls() {
                document.querySelectorAll('a[href]').forEach(link => {
                    const href = link.getAttribute('href');
                    if (href && href.startsWith('uniwebview://open_url?url=')) {
                        const newUrl = href.replace('uniwebview://open_url?url=', '');
                        link.setAttribute('href', newUrl);
                    }
                });
            }
    
            document.addEventListener('click', function(event) {
                 let target = event.target;
                 while (target && target.tagName !== 'A') {
                     target = target.parentNode;
                 }
                 if (target && target.tagName === 'A') {
                     const href = target.getAttribute('href');
                     if (href && href.startsWith('javascript:miHoYoGameJSSDK.openInBrowser')) {
                         event.preventDefault();
                         const regex = /javascript:miHoYoGameJSSDK\.openInBrowser\('([^']+)'\)/;
                         const match = href.match(regex);
                         if (match && match[1]) {
                             window.location.href = "inner:" + match[1];
                         }
                     }
                 }
            });
        };
        """;

    private const string AnnouncementUrl =
        "https://sdk.mihoyo.com/nap/announcement/index.html?auth_appid=announcement&authkey_ver=1&bundle_id=nap_cn&channel_id=1&game=nap&game_biz=nap_cn&lang=zh-cn&level=60&platform=pc&region=prod_gf_cn&sdk_presentation_style=fullscreen&sdk_screen_transparent=true&sign_type=2&uid=100000000&version=2.27#/";

    public void Navigated()
    {

    }

    [ObservableProperty] private string? _announcementHtmlContent;

    public void GameAnnouncementWebView_OnInitialized(object? sender, EventArgs e)
    {
        Announcements.AnnouncementWebView.Source = new Uri(AnnouncementUrl);
        Log.Information("[Announcements] WebView initialized");
    }

    public void GameAnnouncementWebView_OnNavigationStarting(object? sender, WebViewNavigationStartingEventArgs e)
    {
        if (!e.Request!.ToString().StartsWith("inner:")) return;
        
        PlatformHelper.OpenUrl(e.Request!.ToString()[6..]);
        e.Cancel = true;
        Log.Information("[Announcements] Open inner URL: {Url}", e.Request!.ToString()[6..]);
    }

    public async void GameAnnouncementWebView_OnDomContentLoaded(object? sender, WebViewDomContentLoadedEventArgs e)
    {
        await Announcements.AnnouncementWebView.InvokeScript(Script);
    }
}