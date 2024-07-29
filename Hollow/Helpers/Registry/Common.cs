using System.Runtime.Versioning;

namespace Hollow.Helpers.Registry;

[SupportedOSPlatform("Windows")]
public static class Common
{
    public static int? GetRegDword(string key, string value)
    {
        return (int?)Microsoft.Win32.Registry.GetValue(key, value, null);
    }
    
    public static void SetRegDword(string key, string value, int data)
    {
        Microsoft.Win32.Registry.SetValue(key, value, data, Microsoft.Win32.RegistryValueKind.DWord);
    }
}