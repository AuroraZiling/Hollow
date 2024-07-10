using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Core.Gacha.Uigf;
using Hollow.Models;

namespace Hollow.Services.GachaService;

public interface IGachaService
{
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfileDictionary { get; set; }
    public Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfileDictionary();
    public Task<Response<GachaRecords>> GetGachaRecords(string authKey, IProgress<Response<string>> progress);
    public Response<string> GetAuthKey();
}