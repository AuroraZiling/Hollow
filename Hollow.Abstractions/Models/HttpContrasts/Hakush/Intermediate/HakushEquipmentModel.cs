using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.Hakush.Intermediate;

public class HakushEquipmentModel
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";
    [JsonPropertyName("EN")]
    public HakushEquipmentLanguageModel English { get; set; } = new();
    [JsonPropertyName("CHS")]
    public HakushEquipmentLanguageModel ChineseSimplified { get; set; } = new();
}

public class HakushEquipmentLanguageModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("desc2")]
    public string Description { get; set; } = "";
    [JsonPropertyName("desc4")]
    public string DetailedDescription { get; set; } = "";
}