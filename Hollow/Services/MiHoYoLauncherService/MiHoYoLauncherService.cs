using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hollow.Core.MiHoYoLauncher;
using Hollow.Core.MiHoYoLauncher.Models;
using Hollow.Helpers;
using Hollow.Helpers.Announcement;
using Hollow.Models.Pages.Announcement;
using Hollow.Services.ConfigurationService;

namespace Hollow.Services.MiHoYoLauncherService;

public class MiHoYoLauncherService(HttpClient httpClient, IConfigurationService configurationService) : IMiHoYoLauncherService
{
    private const string GameInfoUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGames?launcher_id={LauncherId.CnOfficial}";

    public async Task<ZzzGameInfo?> GetGameInfo()
    {
        var response = await httpClient.GetStringAsync(GameInfoUrl);
        return JsonSerializer.Deserialize<ZzzGameInfo>(response);
    }
    
    private const string AllGameBasicInfoUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id={LauncherId.CnOfficial}&game_id={GameId.Zzz}";
    private ZzzAllGameBasicInfo? _cachedAllGameBasicInfo;

    public async Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo()
    {
        if (_cachedAllGameBasicInfo is not null)
        {
            return _cachedAllGameBasicInfo;
        }
        var response = await httpClient.GetStringAsync(AllGameBasicInfoUrl);
        _cachedAllGameBasicInfo = JsonSerializer.Deserialize<ZzzAllGameBasicInfo>(response);
        return _cachedAllGameBasicInfo;
    }
    
    private const string GameContentUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGameContent?launcher_id={LauncherId.CnOfficial}&game_id={GameId.Zzz}";

    public async Task<ZzzGameContent?> GetGameContent()
    {
        var response = await httpClient.GetStringAsync(GameContentUrl);
        return JsonSerializer.Deserialize<ZzzGameContent>(response);
    }
    
    //TODO: UID
    private const string AnnouncementUrl = "https://announcement-api.mihoyo.com/common/nap_cn/announcement/api/getAnnList?game=nap&game_biz=nap_cn&lang=zh-cn&bundle_id=nap_cn&channel_id=1&platform=pc&region=prod_gf_cn&uid=10000000";
    private const string AnnouncementContentUrl = "https://announcement-static.mihoyo.com/common/nap_cn/announcement/api/getAnnContent?game=nap&game_biz=nap_cn&lang=zh-cn&bundle_id=nap_cn&platform=pc&region=prod_gf_cn&level=60&uid=10000000";

    public async Task<Dictionary<int, List<AnnouncementModel>>?> GetAnnouncement()
    {
        var announcement = JsonSerializer.Deserialize<ZzzAnnouncement>(await httpClient.GetStringAsync(AnnouncementUrl));
        var announcementContent = JsonSerializer.Deserialize<ZzzAnnouncementContent>(await httpClient.GetStringAsync(AnnouncementContentUrl));
        
        if (announcement is null || announcementContent is null)
        {
            return null;
        }

        var announcementResult = new Dictionary<int, List<AnnouncementModel>>
        {
            { 3, [] },
            { 4, [] }
        };
        
        var announcementContentPair = announcementContent.Data.List.ToDictionary(
            item => item.Id,
            item => item.Content
        );
        var announcementList = announcement.Data.List;

        foreach (var announcementListInType in announcementList)
        {
            if(announcementListInType.TypeId != 3 && announcementListInType.TypeId != 4)
            {
                continue;
            }
            foreach (var announcementItem in announcementListInType.AnnouncementList)
            {
                announcementResult[announcementListInType.TypeId].Add(new AnnouncementModel
                {
                    Id = announcementItem.Id,
                    Title = HtmlFilter.RemoveP(announcementItem.Title),
                    Subtitle = HtmlFilter.RemoveBr(announcementItem.Subtitle),
                    BannerUrl = announcementItem.BannerUrl,
                    Content = HtmlHelper.GetHtml(announcementContentPair[announcementItem.Id]),
                    TagLabel = announcementItem.TagLabel,
                    TagIconUrl = announcementItem.TagIconUrl,
                    TagIconHoverUrl = announcementItem.TagIconHoverUrl,
                    StartTime = announcementItem.StartTime,
                    EndTime = announcementItem.EndTime,
                    HasContent = announcementItem.HasContent
                });
            }
        }

        return announcementResult;
    }
}