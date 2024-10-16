﻿using System.Reflection;

namespace Hollow.Abstractions.Models;

public class AppInfo
{
    public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

    public static readonly string BasePath = Directory.GetCurrentDirectory();
    public static readonly string ConfigPath = Path.Combine(BasePath, "config.json");
    public static readonly string LogDir = Path.Combine(BasePath, "logs");
    public static readonly string CachesDir = Path.Combine(BasePath, "caches");
    public static readonly string MetadataDir = Path.Combine(BasePath, "metadata");
    public static readonly string GachaRecordPath = Path.Combine(BasePath, "records.json");
}