using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Hollow.Helpers;

public static class StorageHelper
{
    public static async Task<string> OpenFolderPickerForPath()
    {
        var dialog = await new Window().StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {AllowMultiple = false});
        return dialog.Count == 0 ? string.Empty : Uri.UnescapeDataString(dialog[0].Path.AbsolutePath);
    }

    public static void OpenFolderInExplorer(string directoryPath)
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