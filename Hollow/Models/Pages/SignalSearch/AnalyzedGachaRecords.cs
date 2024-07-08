using System.Collections.Generic;

namespace Hollow.Models.Pages.SignalSearch;

public class AnalyzedGachaRecords
{
    public required AnalyzedCommonGachaRecord StandardGachaRecords { get; set; }
    public required AnalyzedCommonGachaRecord ExclusiveGachaRecords { get; set; }
    public required AnalyzedCommonGachaRecord WEngineGachaRecords { get; set; }
    public required AnalyzedCommonGachaRecord BangbooGachaRecords { get; set; }
}

public class AnalyzedCommonBasicGachaRecord
{
    public int Total { get; set; }
    public double TotalAverage { get; set; }
    
    public int TotalS { get; set; }
    public double SPercentage { get; set; }
    
    public int TotalA { get; set; }
    public double APercentage { get; set; }
    
    public int TotalB { get; set; }
    public double BPercentage { get; set; }
    
    public required string TimeRange { get; set; }
}

public class AnalyzedCommonGachaRecordItem
{
    // Original
    public required string ItemId { get; set; }
    public required string Time { get; set; }
    public required string Name { get; set; }
    public required string ItemType { get; set; }
    public required string RankType { get; set; }
    public required string Id { get; set; }
    
    // Additional
    public required long Timestamp { get; set; }
    public required bool IsSinglePoll { get; set; }
    public required int NthPoll { get; set; } // If IsSinglePoll is true, this should be 1
    public required int NthGuaranteePoll { get; set; }
}

public class AnalyzedCommonGachaRecord
{
    public required AnalyzedCommonBasicGachaRecord BasicInfo { get; set; }
    public required List<AnalyzedCommonGachaRecordItem> Items { get; set; }
}