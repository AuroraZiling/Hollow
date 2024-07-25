using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;

namespace Hollow.Services.GachaService;

public interface IGachaService
{
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }
    public Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary();
    public Task<bool> IsAuthKeyValid(string authKey);
    public Task<Response<GachaRecords>> GetGachaRecords(string authKey, IProgress<Response<string>> progress);
    public Response<string> GetAuthKey();
    public Response<string> GetAuthKeyFromUrl(string url);
}