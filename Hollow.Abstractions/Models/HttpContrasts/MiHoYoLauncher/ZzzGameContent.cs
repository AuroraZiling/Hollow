using System.Text.Json.Serialization;
using Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher.Common;

namespace Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher;


public class ZzzGameContent
{
    [JsonPropertyName("data")]
    public required ZzzGameContentData Data { get; set; }
}

public class ZzzGameContentData
{
    [JsonPropertyName("content")]
    public required ZzzGameContentDataContent Content { get; set; }
}

public class ZzzGameContentDataContent
{
    [JsonPropertyName("banners")]
    public required List<ZzzGameContentDataContentBanner> Banners { get; set; }
    [JsonPropertyName("posts")]
    public required List<ZzzGameContentDataContentPost> Posts { get; set; }
}

public class ZzzGameContentDataContentBanner
{
    [JsonPropertyName("image")]
    public required ImageModel Image { get; set; }
}


public class ZzzGameContentDataContentPost
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    [JsonPropertyName("link")]
    public required string Link { get; set; }
    
    [JsonPropertyName("date")]
    public required string Date { get; set; }
}