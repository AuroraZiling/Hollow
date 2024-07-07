using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Services.MiHoYoLauncherService;

namespace Hollow.ViewModels.Pages;

public partial class AnnouncementsViewModel(IMiHoYoLauncherService miHoYoLauncherService)
    : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
    }

    [ObservableProperty] private string _announcementUrl =
        "https://sdk.mihoyo.com/nap/announcement/index.html?auth_appid=announcement&authkey_ver=1&bundle_id=nap_cn&channel_id=1&game=nap&game_biz=nap_cn&lang=zh-cn&level=60&platform=pc&region=prod_gf_cn&sdk_presentation_style=fullscreen&sdk_screen_transparent=true&sign_type=2&uid=100000000&version=2.27#/";
}