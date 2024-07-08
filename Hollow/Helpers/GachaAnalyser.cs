using System;
using System.Collections.Generic;
using System.Linq;
using Hollow.Core.Gacha.Common;
using Hollow.Core.Gacha.Uigf;
using Hollow.Models.Pages.SignalSearch;

namespace Hollow.Helpers;

public static class GachaAnalyser
{
    public static AnalyzedGachaRecords FromGachaRecords(GachaRecords gachaRecords)
    {
        // Separate gacha records by gacha type
        var standardGachaRecords = new List<GachaItem>();
        var exclusiveGachaRecords = new List<GachaItem>();
        var wEngineGachaRecords = new List<GachaItem>();
        var bangbooGachaRecords = new List<GachaItem>();

        foreach (var gachaItem in gachaRecords.List)
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
        
        // Analyze gacha records
        return new AnalyzedGachaRecords
        {
            StandardGachaRecords = FromGachaItems(standardGachaRecords),
            ExclusiveGachaRecords = FromGachaItems(exclusiveGachaRecords),
            WEngineGachaRecords = FromGachaItems(wEngineGachaRecords),
            BangbooGachaRecords = FromGachaItems(bangbooGachaRecords),
        };
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
                NthPoll = 0, // Not initialized yet
                NthGuaranteePoll = 0, // Not initialized yet
            });
        }

        analyzedCommonGachaRecordItems = analyzedCommonGachaRecordItems.OrderByDescending(item => item.Timestamp).ToList();
        
        // Analyze
        var total = analyzedCommonGachaRecordItems.Count;
        var totalS = 0;
        var totalA = 0;
        var totalB = 0;

        // Calculate 1st
        var guaranteeCounter = 0;
        for (var gachaItemIndex = total - 1; gachaItemIndex >= 0; gachaItemIndex--)
        {
            guaranteeCounter++;
            analyzedCommonGachaRecordItems[gachaItemIndex].NthGuaranteePoll = guaranteeCounter;
            
            switch (analyzedCommonGachaRecordItems[gachaItemIndex].RankType)
            {
                case "4":
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
        // Calculate 10 pulls
        for (var gachaItemIndex = total - 1; gachaItemIndex >= 10; gachaItemIndex--)
        {
            if (analyzedCommonGachaRecordItems[gachaItemIndex].Timestamp !=
                analyzedCommonGachaRecordItems[gachaItemIndex - 1].Timestamp) continue;
            for (var i = gachaItemIndex; i > gachaItemIndex-10; i--)
            {
                analyzedCommonGachaRecordItems[i].NthPoll = -i + gachaItemIndex + 1;
                analyzedCommonGachaRecordItems[i].IsSinglePoll = false;
            }
            gachaItemIndex -= 9;
        }
        
        var totalAverage = total / (double)totalS;
        var sPercentage = Math.Round(totalS / (double)total * 100, 2);
        var aPercentage = Math.Round(totalA / (double)total * 100, 2);
        var bPercentage = Math.Round(totalB / (double)total * 100, 2);
            
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
            Items = analyzedCommonGachaRecordItems
        };
    }

    private static long GetTimestamp(string time)
        => (DateTime.Parse(time).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
}