namespace Hollow.Services.GameService;

public interface IGameService
{
    public string GetGameVersion();
    public bool StartGame();
}