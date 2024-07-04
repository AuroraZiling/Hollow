using System.Text.Json.Serialization;
using Hollow.Core.MiHoYoLauncher.Models.Common;

namespace Hollow.Core.MiHoYoLauncher.Models;

public class ZzzGameInfo
{
    [JsonPropertyName("data")]
    public required ZzzGameInfoDataModel Data { get; set; }
}

public class ZzzGameInfoDataModel
{
    [JsonPropertyName("games")]
    public required List<ZzzGameInfoGameModel> Games { get; set; }
}

public class ZzzGameInfoGameModel
{
    [JsonPropertyName("id")]
    public required string GameId { get; set; }
    [JsonPropertyName("biz")]
    public required string GameBiz { get; set; }
    [JsonPropertyName("display")]
    public required ZzzGameInfoGameDisplayModel Display { get; set; }
}

public class ZzzGameInfoGameDisplayModel
{
    [JsonPropertyName("name")]
    public required string GameName { get; set; }
    [JsonPropertyName("background")]
    public required ImageModel Image { get; set; }
}