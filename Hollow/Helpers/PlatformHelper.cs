using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Hollow.Abstractions.Models;
using Serilog;

namespace Hollow.Helpers;


//TODO: Platform specific
public static class PlatformHelper
{
    public static async Task<string> OpenFolderPickerForPath()
    {
        var dialog = await new Window().StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {AllowMultiple = false, Title = "\"ZenlessZoneZero Game\" Folder"});
        return dialog.Count == 0 ? string.Empty : Uri.UnescapeDataString(dialog[0].Path.AbsolutePath);
    }
    public static void OpenFolderInExplorer(string directoryPath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var directory = new Uri(directoryPath);
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = directory.LocalPath
            });
            process?.WaitForExit();
        }
    }
    
    private static long GetDirectorySize(string folderPath)
    {
        var directory = new DirectoryInfo(folderPath);
        return directory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
    }
    
    private static double ConvertBytesToMegabytes(long bytes)
    {
        return Math.Round((double)bytes / (1024 * 1024), 2);
    }
    
    public static double GetCacheFolderMegabytes()
    {
        return ConvertBytesToMegabytes(GetDirectorySize(AppInfo.CachesDir));
    }
    
    public static void ClearCacheFolder()
    {
        try
        {
            Directory.Delete(AppInfo.CachesDir, true);
            Directory.CreateDirectory(AppInfo.CachesDir);
        }catch (IOException e)
        {
            Log.Error(e, "Failed to clear cache file");
        }
    }
    
    public static void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}