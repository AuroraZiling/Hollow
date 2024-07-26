using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;

public class GachaItem
{
    [JsonPropertyName("uid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // This field used for getting UID from 1st page, UIGF does not have this field
    public string? Uid { get; set; }

    [JsonPropertyName("gacha_id")] 
    public string GachaId { get; set; } = "0";
    
    [JsonPropertyName("gacha_type")]
    public required string GachaType { get; set; }
    
    [JsonPropertyName("item_id")]
    public required string ItemId { get; set; }

    [JsonPropertyName("count")] 
    public string Count { get; set; } = "1"; 
    
    [JsonPropertyName("time")]
    public required string Time { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }  // TODO: UIGF 4.0: Not required
    
    [JsonPropertyName("item_type")]
    public required string ItemType { get; set; }  // TODO: UIGF 4.0: Not required
    
    [JsonPropertyName("rank_type")]
    public required string RankType { get; set; }  // TODO: UIGF 4.0: Not required
    
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}