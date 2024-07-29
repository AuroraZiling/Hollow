using System;
using System.IO;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hollow.Abstractions.Models;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GachaService;
using Hollow.Services.GameService;
using Hollow.Services.MetadataService;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;
using Hollow.ViewModels;
using Hollow.ViewModels.Pages;
using Hollow.Views;
using Hollow.Views.Pages;
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
        
        AvaloniaXamlLoader.Load(this);
    }
    
    public static void ConfigureServices(Action<IServiceCollection>? customServicesFactory = null)
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        
        services.AddSingleton<Home>();
        services.AddSingleton<HomeViewModel>();
        
        services.AddSingleton<Announcements>();
        services.AddSingleton<AnnouncementsViewModel>();
        
        services.AddSingleton<GameSettings>();
        services.AddSingleton<GameSettingsViewModel>();
        
        services.AddSingleton<SignalSearch>();
        services.AddSingleton<SignalSearchViewModel>();
        
        services.AddSingleton<Wiki>();
        services.AddSingleton<WikiViewModel>();
        
        services.AddSingleton<Settings>();
        services.AddSingleton<SettingsViewModel>();

        services.AddSingleton<HttpClient>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMiHoYoLauncherService, MiHoYoLauncherService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IGachaService, GachaService>();
        services.AddSingleton<IMetadataService, MetadataService>();
        
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

        base.OnFrameworkInitializationCompleted();
    }
}