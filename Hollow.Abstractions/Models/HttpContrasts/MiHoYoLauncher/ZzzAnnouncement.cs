using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;

public class ZzzAnnouncement
{
    [JsonPropertyName("data")]
    public required ZzzAnnouncementDataModel Data { get; set; }
}

public class ZzzAnnouncementDataModel
{
    [JsonPropertyName("list")]
    public required List<ZzzAnnouncementInfoModel> List { get; set; }
}

public class ZzzAnnouncementInfoModel
{
    [JsonPropertyName("list")]
    public required List<ZzzAnnouncementModel> AnnouncementList { get; set; }
    
    [JsonPropertyName("type_id")]
    public required int TypeId { get; set; } // 3: 游戏公告, 4: 活动公告
}

public class ZzzAnnouncementModel
{
    [JsonPropertyName("ann_id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    [JsonPropertyName("subtitle")]
    public required string Subtitle { get; set; }

    [JsonPropertyName("banner")] 
    public required string BannerUrl { get; set; }
    
    [JsonPropertyName("tag_label")] 
    public required string TagLabel { get; set; }
    
    [JsonPropertyName("tag_icon")] 
    public required string TagIconUrl { get; set; }
    
    [JsonPropertyName("tag_icon_hover")] 
    public required string TagIconHoverUrl { get; set; }
    
    [JsonPropertyName("start_time")]
    public required string StartTime { get; set; }
    
    [JsonPropertyName("end_time")]
    public required string EndTime { get; set; }
    
    [JsonPropertyName("has_content")]
    public required bool HasContent { get; set; }
}