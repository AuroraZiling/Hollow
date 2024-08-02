using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.Hakush;

public class HakushCharacterModel
{
    [JsonPropertyName("Icon")]
    public required string Icon { get; set; }
    
    [JsonPropertyName("Name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("CodeName")]
    public required string CodeName { get; set; }
    
    [JsonPropertyName("Rarity")]
    public required int Rarity { get; set; }
    
    [JsonPropertyName("WeaponType")]
    public required Dictionary<string, string> WeaponType { get; set; }
    
    [JsonPropertyName("ElementType")]
    public required Dictionary<string, string> ElementType { get; set; }
    
    [JsonPropertyName("HitType")]
    public required Dictionary<string, string> HitType { get; set; }
    
    [JsonPropertyName("Camp")]
    public required Dictionary<string, string> Camp { get; set; }
    
    [JsonPropertyName("PartnerInfo")]
    public required HakushCharacterPartnerInfoModel PartnerInfo { get; set; }
}

public class HakushCharacterPartnerInfoModel
{
    [JsonPropertyName("Birthday")] public string Birthday { get; set; } = "";
    
    [JsonPropertyName("Gender")] public string Gender { get; set; } = "";
    
    [JsonPropertyName("ImpressionF")] public string ImpressionFemale { get; set; } = "";
    
    [JsonPropertyName("ImpressionM")] public string ImpressionMale { get; set; } = "";
    
    [JsonPropertyName("OutlookDesc")] public string OutlookDescription { get; set; } = "";
    
    [JsonPropertyName("ProfileDesc")] public string ProfileDescription { get; set; } = "";
}