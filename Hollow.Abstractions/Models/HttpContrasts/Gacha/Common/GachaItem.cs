using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;

/// <summary>
/// Smallest unit of gacha record
/// </summary>
public class GachaItem
{
    /// <summary>
    /// Player UID
    /// </summary>
    /// <remarks>China: 8 digits, Global: 10 digits</remarks>
    [JsonPropertyName("uid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // This field used for getting UID from 1st page, UIGF does not have this field, ignore
    public string? Uid { get; set; }

    /// <summary>
    /// Gacha ID
    /// </summary>
    /// <remarks>Probably meaningless, always 0</remarks>
    [JsonPropertyName("gacha_id")] 
    public string GachaId { get; set; } = "0";
    
    /// <summary>
    /// Gacha Type
    /// </summary>
    /// <remarks>1: Standard, 2: Exclusive, 3: W-Engine, 5: Bangboo</remarks>
    [JsonPropertyName("gacha_type")]
    public required string GachaType { get; set; }
    
    /// <summary>
    /// Unique ID for items
    /// </summary>
    [JsonPropertyName("item_id")]
    public required string ItemId { get; set; }

    /// <summary>
    /// Count
    /// </summary>
    /// <remarks>Probably meaningless, always 1</remarks>
    [JsonPropertyName("count")] 
    public string Count { get; set; } = "1";
    
    /// <summary>
    /// Time
    /// </summary>
    /// <remarks>Server time</remarks>
    [JsonPropertyName("time")]
    public required string Time { get; set; }
    
    
    /// <summary>
    /// Item name
    /// </summary>
    /// <remarks>I18N Property</remarks>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Item type
    /// </summary>
    /// <remarks>I18N Property</remarks>
    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = "";
    
    /// <summary>
    /// Rank type
    /// </summary>
    /// <remarks>2: B, 3: A, 4: S</remarks>
    [JsonPropertyName("rank_type")]
    public string RankType { get; set; } = "";
    
    /// <summary>
    /// Record ID
    /// </summary>
    /// <remarks>Unique, always increasing</remarks>
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}