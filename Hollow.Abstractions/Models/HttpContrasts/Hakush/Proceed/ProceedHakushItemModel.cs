﻿namespace Hollow.Abstractions.Models.HttpContrasts.Hakush.Proceed;

public class ProceedHakushItemModel
{
    public bool IsCompleted { get; set; }
    public string Icon { get; set; } = "";
    public int? RankType { get; set; }
    public int? GachaType { get; set; }
    public string EnglishName { get; set; } = "";
    public string ChineseName { get; set; } = "";
    public string ItemType { get; set; } = "";
}