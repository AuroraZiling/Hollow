using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.Hakush;

public class HakushItemModel
{
    [JsonPropertyName("rank")]
    public int? RankType { get; set; }
    [JsonPropertyName("type")]
    public int? GachaType { get; set; }
    [JsonPropertyName("EN")]
    public string EnglishName { get; set; } = "";
    [JsonPropertyName("CHS")]
    public string ChineseName { get; set; } = "";
}