using System.Text.Json.Serialization;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;

namespace Hollow.Abstractions.Models.HttpContrasts.Gacha;

public class GachaPage
{
    [JsonPropertyName("data")]
    public GachaPageData? Data { get; set; }
}

public class GachaPageData
{
    [JsonPropertyName("list")]
    public required List<GachaItem> List { get; set; }
    
    [JsonPropertyName("region")]
    public required string Region { get; set; }
}

