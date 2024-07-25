﻿using Avalonia;
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
        //TODO: Platform specific
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", AppInfo.CachesDir);

        try
        {
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
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}