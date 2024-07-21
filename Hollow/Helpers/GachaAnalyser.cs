using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Models.Pages.SignalSearch;

namespace Hollow.Helpers;

public static class GachaAnalyser
{
    public static Dictionary<string, AnalyzedGachaRecordProfile> FromGachaProfiles(Dictionary<string, GachaRecordProfile> gachaProfiles)
    {
        var analyzed = new Dictionary<string, AnalyzedGachaRecordProfile>();
        
        foreach (var profile in gachaProfiles.Keys)
        {
            // Separate gacha records by gacha type
            var standardGachaRecords = new List<GachaItem>();
            var exclusiveGachaRecords = new List<GachaItem>();
            var wEngineGachaRecords = new List<GachaItem>();
            var bangbooGachaRecords = new List<GachaItem>();
            foreach (var gachaItem in gachaProfiles[profile].List)
            {
                switch (gachaItem.GachaType)
                {
                    case "1":
                        standardGachaRecords.Add(gachaItem);
                        break;
                    case "2":
                        exclusiveGachaRecords.Add(gachaItem);
                        break;
                    case "3":
                        wEngineGachaRecords.Add(gachaItem);
                        break;
                    case "5":
                        bangbooGachaRecords.Add(gachaItem);
                        break;
                }
            }
            analyzed.Add(profile, new AnalyzedGachaRecordProfile
            {
                StandardGachaRecords = FromGachaItems(standardGachaRecords),
                ExclusiveGachaRecords = FromGachaItems(exclusiveGachaRecords),
                WEngineGachaRecords = FromGachaItems(wEngineGachaRecords),
                BangbooGachaRecords = FromGachaItems(bangbooGachaRecords),
            });
        }
        
        return analyzed;
    }

    private static AnalyzedCommonGachaRecord FromGachaItems(List<GachaItem> gachaItems)
    {
        // Prepare analyzed gacha record items
        var analyzedCommonGachaRecordItems = new List<AnalyzedCommonGachaRecordItem>();
        foreach (var gachaItem in gachaItems)
        {
            analyzedCommonGachaRecordItems.Add(new AnalyzedCommonGachaRecordItem
            {
                Id = gachaItem.Id,
                ItemId = gachaItem.ItemId,
                ItemType = gachaItem.ItemType,
                Name = gachaItem.Name,
                RankType = gachaItem.RankType,
                Time = gachaItem.Time,
                Timestamp = GetTimestamp(gachaItem.Time),
                
                IsSinglePoll = true, // Not initialized yet
                NthPull = 0, // Not initialized yet
                NthGuaranteePull = 0, // Not initialized yet
            });
        }

        analyzedCommonGachaRecordItems = analyzedCommonGachaRecordItems.OrderByDescending(item => item.Timestamp).ToList();
        
        // Analyze
        var total = analyzedCommonGachaRecordItems.Count;
        var totalS = 0;
        var totalA = 0;
        var totalB = 0;
        var overviewCardGachaItems = new ObservableCollection<OverviewCardGachaItem>();

        // Calculate 1st
        var guaranteeCounter = 0;
        for (var gachaItemIndex = total - 1; gachaItemIndex >= 0; gachaItemIndex--)
        {
            guaranteeCounter++;
            analyzedCommonGachaRecordItems[gachaItemIndex].NthGuaranteePull = guaranteeCounter;
            
            switch (analyzedCommonGachaRecordItems[gachaItemIndex].RankType)
            {
                case "4":
                    overviewCardGachaItems.Add(new OverviewCardGachaItem
                    {
                        Name = analyzedCommonGachaRecordItems[gachaItemIndex].Name,
                        PollsUsed = guaranteeCounter
                    });
                    guaranteeCounter = 0;
                    totalS++;
                    break;
                case "3":
                    totalA++;
                    break;
                case "2":
                    totalB++;
                    break;
            }
        }
        
        if(guaranteeCounter > 0)
        {
            overviewCardGachaItems.Add(new OverviewCardGachaItem
            {
                Name = "",
                PollsUsed = guaranteeCounter
            });
        }

        overviewCardGachaItems = new ObservableCollection<OverviewCardGachaItem>(overviewCardGachaItems.Reverse());
        
        // Calculate 10 pulls
        for (var gachaItemIndex = total - 1; gachaItemIndex >= 9; gachaItemIndex--)
        {
            if (analyzedCommonGachaRecordItems[gachaItemIndex].Timestamp !=
                analyzedCommonGachaRecordItems[gachaItemIndex - 1].Timestamp) continue;
            for (var i = gachaItemIndex; i > gachaItemIndex-10; i--)
            {
                analyzedCommonGachaRecordItems[i].NthPull = -i + gachaItemIndex + 1;
                analyzedCommonGachaRecordItems[i].IsSinglePoll = false;
            }
            gachaItemIndex -= 9;
        }
        
        var totalAverage = Math.Round(total / (double)totalS, 2);
        if (totalS == 0)
        {
            totalAverage = double.NaN;
        }
        
        var sPercentage = Math.Round(totalS / (double)total * 100, 2);
        var aPercentage = Math.Round(totalA / (double)total * 100, 2);
        var bPercentage = Math.Round(totalB / (double)total * 100, 2);
        if (total == 0)
        {
            sPercentage = aPercentage = bPercentage = 0;
        }
            
        var timeRange = "Unknown";
        if (total > 0)
        {
            timeRange = $"{analyzedCommonGachaRecordItems.Last().Time} - {analyzedCommonGachaRecordItems.First().Time}";
        }
        
        return new AnalyzedCommonGachaRecord
        {
            BasicInfo = new AnalyzedCommonBasicGachaRecord
            {
                Total = total,
                TotalAverage = totalAverage,
                TotalS = totalS,
                SPercentage = sPercentage,
                TotalA = totalA,
                APercentage = aPercentage,
                TotalB = totalB,
                BPercentage = bPercentage,
                TimeRange = timeRange
            },
            OverviewCardGachaItems = overviewCardGachaItems,
            Items = analyzedCommonGachaRecordItems
        };
    }

    public static long GetTimestamp(string time)
        => (DateTime.Parse(time).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
}