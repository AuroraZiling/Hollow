using System.Text.Json.Serialization;

namespace Hollow.Core.Gacha.Common;

public class GachaItem
{
    [JsonPropertyName("uid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // This field used for getting UID from 1st page, UIGF does not have this field
    public string? Uid { get; set; }
    
    [JsonPropertyName("gacha_id")]
    public required string GachaId { get; set; }
    
    [JsonPropertyName("gacha_type")]
    public required string GachaType { get; set; }
    
    [JsonPropertyName("item_id")]
    public required string ItemId { get; set; }
    
    [JsonPropertyName("count")]
    public required string Count { get; set; }
    
    [JsonPropertyName("time")]
    public required string Time { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("item_type")]
    public required string ItemType { get; set; }
    
    [JsonPropertyName("rank_type")]
    public required string RankType { get; set; }
    
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}