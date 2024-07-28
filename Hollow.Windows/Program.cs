using System.Runtime.InteropServices;
using Avalonia;
using Hollow.Abstractions.Models;
using Hollow.Views.Controls.WebView;
using Hollow.Windows.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hollow.Windows;

public static class Program
{
    
#if WINDOWS
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();
#endif
    
    
    [STAThread]
    public static void Main(string[] args)
    {
        var showConsole = false;
        
        //TODO: Platform specific
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);

            if(args.Contains("--console"))
                showConsole = true;
        }

        try
        {
            if (showConsole)
            {
                AllocConsole();
            }
            
            App.ConfigureServices(x =>
                x.AddTransient<IWebViewAdapter, WebView2Adapter>()
            );
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Oops, Hollow crashed!");
        }
        finally
        {
            Log.CloseAndFlush();

            if (showConsole)
            {
                FreeConsole();
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect();
}