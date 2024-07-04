using System.Text.Json.Serialization;

namespace Hollow.Models.Configs;

public class AppConfig
{
    [JsonPropertyName("game")]
    public GameConfig Game { get; set; } = new();
    
    [JsonPropertyName("after_executing")]
    public AfterExecutingConfig AfterExecuting { get; set; } = new();
    
    [JsonPropertyName("language")]
    public string Language { get; set; } = "Auto";
    
    [JsonPropertyName("check_updates")]
    public bool CheckUpdates { get; set; }
}

public class GameConfig
{
    [JsonPropertyName("directory")]
    public string Directory { get; set; } = "";
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = "";
}

public class AfterExecutingConfig
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = "minimize";
    
    [JsonPropertyName("keep_front")]
    public bool KeepFront { get; set; }
}