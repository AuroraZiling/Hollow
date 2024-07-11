using System.Text.Json.Serialization;
using Hollow.Core.Gacha.Common;
using Hollow.Core.Helpers;

namespace Hollow.Core.Gacha.Uigf;

public class GachaRecords
{
    [JsonPropertyName("info")]
    public GachaRecordsInfo Info { get; init; } = new();
    
    [JsonPropertyName("nap")]
    public List<GachaRecordProfile> Profiles { get; init; } = [];
}

public class GachaRecordProfile
{
    [JsonPropertyName("uid")]
    [JsonConverter(typeof(JsonIntToStringConverter))]
    public string Uid { get; set; } = "";
    
    [JsonPropertyName("lang")]
    public string Language { get; } = "zh-cn";
    
    [JsonPropertyName("timezone")]
    public uint Timezone { get; } = 8;
    
    [JsonPropertyName("list")]
    public List<GachaItem> List { get; set; } = [];
}

public class GachaRecordsInfo
{
    [JsonPropertyName("export_timestamp")]
    public long ExportTimestamp { get; set; }
    
    [JsonPropertyName("export_app")]
    public string ExportApp { get; } = "Hollow";
    
    [JsonPropertyName("export_app_version")]
    public string ExportAppVersion { get; set; } = "1.0.0";
    
    // TODO: UIGF 4.0 Support
    [JsonPropertyName("version")]
    public string UigfVersion { get; } = "v4.0 READY";
}