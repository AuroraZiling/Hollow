using System;
using System.IO;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Hollow.Abstractions.Models;
using Hollow.Extensions;
using Hollow.ViewModels;
using Hollow.Views;
using Hollow.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hollow;

public class App : Application
{
    private static IServiceProvider? _provider;
    
    public override void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDir, "log_.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        
        Log.Information("[App] Hollow is starting");

        ImageLoader.AsyncImageLoader = new DiskCachedWebImageLoader(AppInfo.CachesDir);
        
        AvaloniaXamlLoader.Load(this);
    }
    
    public static void ConfigureServices(Action<IServiceCollection>? customServicesFactory = null)
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        
        services.AddHollowViews();
        services.AddHollowViewModels();
        services.AddHollowServices();
        
        customServicesFactory?.Invoke(services);

        _provider = services.BuildServiceProvider();
        Log.Information("[App] Services Configured");
    }
    
    public static MainWindow MainWindowInstance { get; private set; } = null!;

    public static T GetService<T>() where T : notnull => (_provider ?? throw new InvalidOperationException("Services not Configured")).GetRequiredService<T>();

    public override void OnFrameworkInitializationCompleted()
    {
        MainWindowInstance = GetService<MainWindow>();
        MainWindowInstance.DataContext = GetService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindowInstance;
        }
        
        HollowHost.NotificationManager = new WindowNotificationManager(MainWindowInstance) { MaxItems = 5, Position = NotificationPosition.BottomRight};

        base.OnFrameworkInitializationCompleted();
    }
}