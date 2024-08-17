using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hollow.Enums;
using Hollow.Languages;
using Hollow.Services.ConfigurationService;
using Serilog;

namespace Hollow.Services.GameService;

public class GameService(IConfigurationService configurationService): IGameService
{
    public bool ValidateGameDirectory(string directoryPath)
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
            try
            {
                var configIniFile = File.ReadAllLines(files.First(file => file.EndsWith("config.ini")));
                GameVersion = configIniFile.First(line => line.StartsWith("game_version=")).Split("=")[1];
                GameBiz = configIniFile.First(line => line.StartsWith("cps=")).Split("=")[1] switch
                {
                    "mihoyo" => GameServer.China,
                    "hoyoverse" => GameServer.Global,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Log.Information("[GameService] Game directory validated ({path})", directoryPath);
            }
            catch (Exception)
            {
                GameVersion = Lang.Service_Game_Unknown;
                GameBiz = GameServer.Unknown;
                Log.Error("[GameService] Game directory validation failed ({path})", directoryPath);
            }
        }
        else
        {
            GameVersion = Lang.Service_Game_Unknown;
            GameBiz = GameServer.Unknown;
            Log.Error("[GameService] Game directory validation failed ({path})", directoryPath);
        }
        return validation;
    }

    public string GameVersion { get; set; } = Lang.Service_Game_Unknown;
    public GameServer GameBiz { get; set; } = GameServer.Unknown;
    
    public bool StartGame()
    {
        try
        {
            var gamePath = configurationService.AppConfig.Game.Directory;
            var gameArguments = configurationService.AppConfig.Game.Arguments;
            var gameExe = Directory.GetFiles(gamePath).First(file => file.EndsWith("ZenlessZoneZero.exe"));
        
            var process = new Process { StartInfo = { Arguments = gameArguments, UseShellExecute = true, FileName = gameExe, CreateNoWindow = true, Verb = "runas" } };
            process.Start();
            
            Log.Information("[GameService] Game started (Arguments: {gameArguments})", gameArguments);
            return true;
        }
        catch(Win32Exception e)
        {
            Log.Error(e, "[GameService] Game start failed");
            return false;
        }
    }
}