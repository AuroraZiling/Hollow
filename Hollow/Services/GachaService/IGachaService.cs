using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Core.Gacha.Uigf;
using Hollow.Models;

namespace Hollow.Services.GachaService;

public interface IGachaService
{
    public Dictionary<string, GachaRecordProfile>? GachaRecordProfiles { get; set; }
    public Task<Dictionary<string, GachaRecordProfile>?> LoadGachaRecordProfiles();
    public Task<Response<GachaRecords>> TryGetGachaLogs(string authKey, IProgress<Response<string>> progress);
    public Response<string> TryGetAuthKey();
}