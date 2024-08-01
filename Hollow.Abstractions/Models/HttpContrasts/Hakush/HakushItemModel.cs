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
    public int? GachaType { get; set; }
    [JsonPropertyName("EN")]
    public string EnglishName { get; set; } = "";
    [JsonPropertyName("CHS")]
    public string ChineseName { get; set; } = "";
    
    // Equipment Specific Fields
    public string EquipmentDescription { get; set; } = "";
    public string EquipmentDetailedDescription { get; set; } = "";
    
    // Proceed Fields
    public bool IsCompleted { get; set; }
    public HakushItemType ItemType { get; set; } = HakushItemType.Unknown;
}