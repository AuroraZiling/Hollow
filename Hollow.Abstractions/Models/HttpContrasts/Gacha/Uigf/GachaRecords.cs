﻿using System.Text.Json.Serialization;
using Hollow.Abstractions.JsonConverters.Converters;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;

namespace Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;

/// <summary>
/// UIGF v4.0 Exported Gacha Records
/// </summary>
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
    public int Timezone { get; set; } = 8;
    
    [JsonPropertyName("list")]
    public List<GachaItem> List { get; set; } = [];
}

public class GachaRecordsInfo
{
    [JsonPropertyName("export_timestamp")]
    [JsonConverter(typeof(JsonIntToStringConverter))]
    public string ExportTimestamp { get; set; } = "";
    
    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = "";
    
    [JsonPropertyName("export_app_version")]
    public string ExportAppVersion { get; set; } = AppInfo.AppVersion;
    
    [JsonPropertyName("version")]
    public string UigfVersion { get; } = "v4.0";
}