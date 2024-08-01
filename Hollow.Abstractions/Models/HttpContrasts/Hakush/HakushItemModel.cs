using System.Text.Json.Serialization;
using Hollow.Abstractions.Enums.Hakush;

namespace Hollow.Abstractions.Models.HttpContrasts.Hakush;

public class HakushItemModel
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";
    [JsonPropertyName("rank")]
    public int? RankType { get; set; }
    [JsonPropertyName("type")]
    public int? ItemPropertyType { get; set; }
    [JsonPropertyName("EN")]
    public string EnglishName { get; set; } = "";
    [JsonPropertyName("CHS")]
    public string ChineseName { get; set; } = "";
    
    // Equipment Specific Fields
    public string EquipmentDescription { get; set; } = "";
    public string EquipmentDetailedDescription { get; set; } = "";
    
    // Character Specific Fields
    [JsonPropertyName("element")]
    public int? CharacterElement { get; set; }
    public string CharacterElementIconRes { get; set; } = "";  // 200: Physical, 201: Fire, 202: Ice, 203: Electric, 205: Ether
    
    // Proceed Fields
    public bool IsCompleted { get; set; }
    public HakushItemType ItemType { get; set; } = HakushItemType.Unknown;
    public string TypeIconRes { get; set; } = "";  // 1: Attack, 2: Stun, 3: Anomaly, 4: Support, 5: Defense
}