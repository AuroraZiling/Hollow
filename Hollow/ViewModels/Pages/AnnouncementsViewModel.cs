using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Controls.WebView;
using Hollow.Helpers;
using Hollow.Views.Pages;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel
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
                                       bodyhome[0].style.transform = "scale(1.33, 1.37)";
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
                                   
                                   // 重写所有 uniwebview URL 方案
                                   function rewriteUniWebViewUrls() {
                                       document.querySelectorAll('a[href]').forEach(link => {
                                           let href = link.getAttribute('href');
                                           if (href && href.startsWith('uniwebview://open_url?url=')) {
                                               let newUrl = href.replace('uniwebview://open_url?url=', '');
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
                                            let href = target.getAttribute('href');
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
    }

    public void GameAnnouncementWebView_OnNavigationStarting(object? sender, WebViewNavigationStartingEventArgs e)
    {
        if (e.Request!.ToString().StartsWith("inner:"))
        {
            HtmlHelper.OpenUrl(e.Request!.ToString()[6..]);
        }
    }

    public async void GameAnnouncementWebView_OnDomContentLoaded(object? sender, WebViewDomContentLoadedEventArgs e)
    {
        await Announcements.AnnouncementWebView.InvokeScript(Script);
    }
}