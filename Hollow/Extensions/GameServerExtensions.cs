using Hollow.Abstractions.Enums;
using Hollow.Languages;

namespace Hollow.Extensions;

public static class GameServerExtensions
{
    public static string ToI18NString(this GameServer server)
    {
        return server switch
        {
            GameServer.China => Lang.Service_GameBiz_China,
            GameServer.Global => Lang.Service_GameBiz_Global,
            _ => Lang.Service_Game_Unknown
        };
    }
}