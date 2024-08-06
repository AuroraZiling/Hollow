using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Common;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Models.SignalSearch;

namespace Hollow.Helpers;

public static class GachaAnalyser
{
    public static Dictionary<string, AnalyzedGachaRecordProfile> FromGachaProfiles(Dictionary<string, GachaRecordProfile> gachaProfiles)
    {
        var analyzed = new Dictionary<string, AnalyzedGachaRecordProfile>();
        
        foreach (var profile in gachaProfiles.Keys)
        {
            var timezoneAdjuster = new TimeZoneAdjuster(gachaProfiles[profile].Timezone);
            
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
                DisplayTimezone = gachaProfiles[profile].Timezone.ToUtcPrefixTimeZone(),
                StandardGachaRecords = FromGachaItems(standardGachaRecords, timezoneAdjuster),
                ExclusiveGachaRecords = FromGachaItems(exclusiveGachaRecords, timezoneAdjuster),
                WEngineGachaRecords = FromGachaItems(wEngineGachaRecords, timezoneAdjuster),
                BangbooGachaRecords = FromGachaItems(bangbooGachaRecords, timezoneAdjuster),
            });
        }
        
        return analyzed;
    }

    private static AnalyzedCommonGachaRecord FromGachaItems(List<GachaItem> gachaItems, TimeZoneAdjuster timezoneAdjuster)
    {
        // Converting original gacha items to analyzed gacha items
        var sortedAnalyzedCommonGachaRecordItems = new SortedSet<AnalyzedCommonGachaRecordItem>(
            Comparer<AnalyzedCommonGachaRecordItem>
                .Create((x, y) => string.Compare(y.Id, x.Id, StringComparison.Ordinal))  // Descending order by Id
        );
        
        foreach (var gachaItem in gachaItems)
        {
            var adjustedTime = timezoneAdjuster.ConvertToLocalTimeZone(gachaItem.Time);
            sortedAnalyzedCommonGachaRecordItems.Add(new AnalyzedCommonGachaRecordItem
            {
                Id = gachaItem.Id,
                ItemId = gachaItem.ItemId,
                ItemType = gachaItem.ItemType,
                Name = gachaItem.Name,
                RankType = gachaItem.RankType,
                Time = adjustedTime,
                Timestamp = GetTimestamp(adjustedTime),
            });
        }

        var analyzedCommonGachaRecordItems = sortedAnalyzedCommonGachaRecordItems.ToList();
        
        // Analyze Total, TotalS, TotalA, TotalB, PollsUsed
        var total = analyzedCommonGachaRecordItems.Count;
        var totalS = 0;
        var totalA = 0;
        var totalB = 0;
        var overviewCardGachaItems = new List<OverviewCardGachaItem>();

        var guaranteeCounter = 0;
        for (var gachaItemIndex = total - 1; gachaItemIndex >= 0; gachaItemIndex--)
        {
            analyzedCommonGachaRecordItems[gachaItemIndex].NthGuaranteePull = ++guaranteeCounter;
            
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

        overviewCardGachaItems.Reverse();
        
        // Calculate Unluckiest and Luckiest
        var unluckiestPulls = 0;
        var luckiestPulls = 0;
        if (overviewCardGachaItems.Count > 0)
        {
            unluckiestPulls = overviewCardGachaItems.Max(item => item.PollsUsed);
            luckiestPulls = overviewCardGachaItems.Min(item => item.PollsUsed);
        }
        
        // Calculate nth pull in a consecutive 10 pulls
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
        
        // Calculate TotalAverage, SPercentage, APercentage, BPercentage
        var totalAverage = Math.Round(total / (double)totalS, 2);
        if (totalS == 0)
        {
            totalAverage = 0;
        }
        
        var sPercentage = Math.Round(totalS / (double)total * 100, 2);
        var aPercentage = Math.Round(totalA / (double)total * 100, 2);
        var bPercentage = Math.Round(totalB / (double)total * 100, 2);
        if (total == 0)
        {
            sPercentage = aPercentage = bPercentage = 0;
        }
            
        // Calculate TimeRange
        var timeRange = "Unknown";
        if (total > 0)
        {
            timeRange = $"{analyzedCommonGachaRecordItems.Last().Time.Split(' ')[0]} - {analyzedCommonGachaRecordItems.First().Time.Split(' ')[0]}";
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
                TimeRange = timeRange,
                UnluckiestPulls = unluckiestPulls,
                LuckiestPulls = luckiestPulls
            },
            OverviewCardGachaItems = new ObservableCollection<OverviewCardGachaItem>(overviewCardGachaItems),
            Items = analyzedCommonGachaRecordItems
        };
    }

    private static long GetTimestamp(string time)
        => (DateTime.Parse(time).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
}