using Avalonia;
using System;
using System.IO;
using Avalonia.WebView.Desktop;
using Hollow.Models;
using Serilog;

namespace Hollow;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDir, "log_.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
            .CreateLogger();
        
        Log.Information("Hollow is starting...");

        //TODO: Platform specific
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);
        
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Oops, Hollow crashed!");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .WithInterFont()
            .UseDesktopWebView();
}