using System;
using System.Runtime.Versioning;
using Hollow.Enums;

namespace Hollow.Helpers.Registry;

[SupportedOSPlatform("Windows")]
public class Screen
{
    private const string ChinaKeyRoot = "HKEY_CURRENT_USER\\Software\\miHoYo\\绝区零";
    private const string GlobalKeyRoot = "HKEY_CURRENT_USER\\Software\\miHoYo\\ZenlessZoneZero";
    
    private const string ResolutionHeightKey = "Screenmanager Resolution Height_h2627697771";
    private const string ResolutionWidthKey = "Screenmanager Resolution Width_h182942802";
    private const string IsFullScreenKey = "Screenmanager Fullscreen mode_h3630240806";
    
    public int ResolutionHeight { get; set; }
    public int ResolutionWidth { get; set; }
    public bool IsFullScreen { get; set; }
    
    public Screen(GameServer gameServer)
    {
        var keyRoot = gameServer switch
        {
            GameServer.China => ChinaKeyRoot,
            GameServer.Global => GlobalKeyRoot,
            _ => throw new ArgumentOutOfRangeException(nameof(gameServer), gameServer, null)
        };
        
        ResolutionHeight = Common.GetRegDword(keyRoot, ResolutionHeightKey) ?? 0;
        ResolutionWidth = Common.GetRegDword(keyRoot, ResolutionWidthKey) ?? 0;
        IsFullScreen = Common.GetRegDword(keyRoot, IsFullScreenKey) == 1;
    }
    
    public static void SaveScreenSettings(GameServer gameServer, int width, int height, bool isFullScreen)
    {
        var keyRoot = gameServer switch
        {
            GameServer.China => ChinaKeyRoot,
            GameServer.Global => GlobalKeyRoot,
            _ => throw new ArgumentOutOfRangeException(nameof(gameServer), gameServer, null)
        };
        
        Common.SetRegDword(keyRoot, ResolutionHeightKey, height);
        Common.SetRegDword(keyRoot, ResolutionWidthKey, width);
        Common.SetRegDword(keyRoot, IsFullScreenKey, isFullScreen ? 1 : 3);
    }
}