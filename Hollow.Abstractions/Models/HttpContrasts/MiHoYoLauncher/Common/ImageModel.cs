using System.Text.Json.Serialization;

namespace Hollow.Abstractions.Models.HttpContrasts.MiHoYoLauncher.Common;

public class ImageModel
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("link")]
    public required string Link { get; set; }
}