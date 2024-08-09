using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Hollow.Abstractions.Models;
using Serilog;

namespace Hollow.Helpers;


public static class PlatformHelper
{
    public static async Task<string> OpenFolderPickerForPath()
    {
        var dialog = await new Window().StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {AllowMultiple = false, Title = "\"ZenlessZoneZero Game\" Folder"});
        return dialog.Count == 0 ? string.Empty : Uri.UnescapeDataString(dialog[0].Path.AbsolutePath);
    }
    
    private static long GetDirectorySize(string folderPath)
    {
        var directory = new DirectoryInfo(folderPath);
        return directory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fileInfo => fileInfo.Length);
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
}