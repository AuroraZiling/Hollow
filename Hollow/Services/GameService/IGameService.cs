using Hollow.Enums;

namespace Hollow.Services.GameService;

public interface IGameService
{
    public bool StartGame();
    public string GameVersion { get; set; }
    public GameServer GameBiz { get; set; }
    public bool ValidateGameDirectory(string directoryPath);
}