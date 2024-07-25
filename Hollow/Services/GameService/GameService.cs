using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hollow.Services.ConfigurationService;
using Serilog;

namespace Hollow.Services.GameService;

public class GameService(IConfigurationService configurationService): IGameService
{
    public static bool ValidateGameDirectory(string directoryPath)
    {
        if (!Path.Exists(directoryPath))
        {
            return false;
        }
        var files = Directory.GetFiles(directoryPath);
        var validation = files.Any(file => file.EndsWith("config.ini")) &&
                         files.Any(file => file.EndsWith("ZenlessZoneZero.exe"));
        if (validation)
        {
            Log.Information("[GameService] Game directory validated ({path})", directoryPath);
        }
        else
        {
            Log.Error("[GameService] Game directory validation failed ({path})", directoryPath);
        }
        return validation;
    }
    
    public string GetGameVersion()
    {
        var files = Directory.GetFiles(configurationService.AppConfig.Game.Directory);
        var configIni = files.First(file => file.EndsWith("config.ini"));
        var version = File.ReadAllLines(configIni).First(line => line.StartsWith("game_version=")).Split("=")[1];
        Log.Information("[GameService] Get game version: {version}", version);
        return version;
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
            
            Log.Information("[GameService] Game started");
            return true;
        }
        catch(Win32Exception)
        {
            Log.Error("[GameService] Game start failed");
            return false;
        }
    }
}