namespace Hollow.Services.GameService;

public interface IGameService
{
    public bool CheckGameStartReady();
    public string GetGameVersion();
    public bool StartGame();
}