using System.Text.Json.Serialization;

namespace Hollow.Core.MiHoYoLauncher.Models;

public class ZzzAnnouncementContent
{
    [JsonPropertyName("data")]
    public required ZzzAnnouncementContentDataModel Data { get; set; }
}

public class ZzzAnnouncementContentDataModel
{
    [JsonPropertyName("list")]
    public required List<ZzzAnnouncementContentModel> List { get; set; }
}

public class ZzzAnnouncementContentModel
{
    [JsonPropertyName("ann_id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    [JsonPropertyName("subtitle")]
    public required string Subtitle { get; set; }

    [JsonPropertyName("banner")] 
    public required string BannerUrl { get; set; }
    
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}