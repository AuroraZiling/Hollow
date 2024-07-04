using System.IO;
using System.Linq;

namespace Hollow.Services.GameService;

public class GameService: IGameService
{
    public static bool ValidateGameDirectory(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath);
        if (files.All(file => !file.EndsWith("config.ini")) || files.All(file => !file.EndsWith("ZenlessZoneZero.exe")))
        {
            return false;
        }
        return true;
    }
}