using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Gacha;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.HttpContrasts.Hakush.Proceed;
using Hollow.Enums;
using Hollow.Models.Pages.SignalSearch;

namespace Hollow.Services.GachaService;

public interface IGachaService
{
    public GachaRecords MergeGachaRecordsFromImport(GachaRecords fileJson, ImportItem[] selectedImportItems, Dictionary<string, ProceedHakushItemModel> itemsMetadata);
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }
    public Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary();
    public Task<bool> IsAuthKeyValid(string authKey);
    public Task<Response<GachaRecords>> GetGachaRecords(GachaUrlData gachaUrlData, IProgress<Response<string>> progress, GameServer gameServer);
    public Response<GachaUrlData> GetGachaUrlData();
    public Response<GachaUrlData> GetGachaUrlDataFromUrl(string url);
}