using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hollow.Core.MiHoYoLauncher;
using Hollow.Core.MiHoYoLauncher.Models;

namespace Hollow.Services.MiHoYoLauncherService;

public class MiHoYoLauncherService(HttpClient httpClient) : IMiHoYoLauncherService
{
    private const string GameInfoUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGames?launcher_id={LauncherId.CnOfficial}";

    public async Task<ZzzGameInfo?> GetGameInfo()
    {
        var response = await httpClient.GetStringAsync(GameInfoUrl);
        return JsonSerializer.Deserialize<ZzzGameInfo>(response);
    }
    
    private const string AllGameBasicInfoUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id={LauncherId.CnOfficial}&game_id={GameId.Zzz}";

    public async Task<ZzzAllGameBasicInfo?> GetAllGameBasicInfo()
    {
        var response = await httpClient.GetStringAsync(AllGameBasicInfoUrl);
        return JsonSerializer.Deserialize<ZzzAllGameBasicInfo>(response);
    }
    
    private const string GameContentUrl = $"https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGameContent?launcher_id={LauncherId.CnOfficial}&game_id={GameId.Zzz}";

    public async Task<ZzzGameContent?> GetGameContent()
    {
        var response = await httpClient.GetStringAsync(GameContentUrl);
        return JsonSerializer.Deserialize<ZzzGameContent>(response);
    }
}