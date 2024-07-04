using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hollow.Services.ConfigurationService;

namespace Hollow.Services.GameService;

public class GameService(IConfigurationService configurationService): IGameService
{
    public static bool ValidateGameDirectory(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath);
        return files.Any(file => file.EndsWith("config.ini")) && files.Any(file => file.EndsWith("ZenlessZoneZero.exe"));
    }
    
    public bool CheckGameStartReady()
    {
        var files = Directory.GetFiles(configurationService.AppConfig.Game.Directory);
        return files.Any(file => file.EndsWith("config.ini")) && files.Any(file => file.EndsWith("ZenlessZoneZero.exe"));
    }
    
    public string GetGameVersion()
    {
        var files = Directory.GetFiles(configurationService.AppConfig.Game.Directory);
        var configIni = files.First(file => file.EndsWith("config.ini"));
        var lines = File.ReadAllLines(configIni);
        return lines.First(line => line.StartsWith("game_version=")).Split("=")[1];
    }
    
    public bool StartGame()
    {
        try
        {
            
            var gamePath = configurationService.AppConfig.Game.Directory;
            var gameArguments = configurationService.AppConfig.Game.Arguments;
            var gameExe = Directory.GetFiles(gamePath).First(file => file.EndsWith("ZenlessZoneZero.exe"));
        
            var process = new Process { StartInfo = { Arguments = gameArguments, UseShellExecute = true, FileName = gameExe, CreateNoWindow = true, Verb = "runas" } };
            process.Start();
            return true;
        }
        catch(Win32Exception)
        {
            return false;
        }
    }
}