using System.Text.Json.Serialization;
using Hollow.Core.MiHoYoLauncher.Models.Common;

namespace Hollow.Core.MiHoYoLauncher.Models;

public class ZzzAllGameBasicInfo
{
    [JsonPropertyName("data")]
    public required ZzzAllGameBasicInfoDataModel Data { get; set; }
}

public class ZzzAllGameBasicInfoDataModel
{
    [JsonPropertyName("game_info_list")]
    public required List<ZzzAllGameBasicInfoEachDataModel> GameInfo { get; set; }
}

public class ZzzAllGameBasicInfoEachDataModel
{
    [JsonPropertyName("backgrounds")]
    public required List<ZzzAllGameBasicInfoEachDataBackgroundModel> Backgrounds { get; set; }
}

public class ZzzAllGameBasicInfoEachDataBackgroundModel
{
    [JsonPropertyName("background")]
    public required ImageModel Image { get; set; }
    
    [JsonPropertyName("icon")]
    public required ImageModel Icon { get; set; }
}