using Windows.Win32;
using Avalonia;
using Hollow.Abstractions.Models;
using Hollow.Views.Controls.WebView;
using Hollow.Windows.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hollow.Windows;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var showConsole = false;
        
        //TODO: Platform specific
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);

        if(args.Contains("--console"))
            showConsole = true;

        try
        {
            if (showConsole)
            {
                PInvoke.AllocConsole();
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
                PInvoke.FreeConsole();
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions
            {
                RenderingMode = [Win32RenderingMode.AngleEgl]
            });
}