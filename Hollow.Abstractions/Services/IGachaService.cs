using Hollow.Abstractions.Enums;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Gacha;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Abstractions.Models.SignalSearch;

namespace Hollow.Abstractions.Services;

public interface IGachaService
{
    public GachaRecords MergeGachaRecordsFromImport(GachaRecords fileJson, ImportItem[] selectedImportItems, Dictionary<string, HakushItemModel> itemsMetadata);
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }
    public Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary();
    public Task<bool> IsAuthKeyValid(string authKey);
    public Task<Response<GachaRecords>> GetGachaRecords(GachaUrlData gachaUrlData, IProgress<Response<string>> progress, GameServer gameServer);
    public Response<GachaUrlData> GetGachaUrlData();
    public Response<GachaUrlData> GetGachaUrlDataFromUrl(string url);
}