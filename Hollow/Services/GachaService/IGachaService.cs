using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Core.Gacha.Uigf;
using Hollow.Models;

namespace Hollow.Services.GachaService;

public interface IGachaService
{
    public Dictionary<string, GachaRecords>? GachaRecords { get; set; }
    public Task<Dictionary<string, GachaRecords>?> LoadGachaRecords();
    public Task<Response<GachaRecords>> TryGetGachaLogs(string authKey, IProgress<Response<string>> progress);
    public Task<Response<string>> TryGetAuthKey();
}