using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hollow.Models.SignalSearch;

public class AnalyzedGachaRecordProfile
{
    public required string DisplayTimezone { get; set; }
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
    
    public int UnluckiestPulls { get; set; }
    public int LuckiestPulls { get; set; }
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
    public bool IsSinglePoll { get; set; } = true;
    public int NthPull { get; set; }  // If IsSinglePull is true, this should be 0
    public int NthGuaranteePull { get; set; }
}

public class AnalyzedCommonGachaRecord
{
    public required AnalyzedCommonBasicGachaRecord BasicInfo { get; set; }
    public required ObservableCollection<OverviewCardGachaItem> OverviewCardGachaItems { get; set; }
    public required List<AnalyzedCommonGachaRecordItem> Items { get; set; }
}