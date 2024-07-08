using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hollow.Models;
using Hollow.Services.ConfigurationService;
using Hollow.Services.GachaService;
using Hollow.Services.GameService;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;
using Hollow.ViewModels;
using Hollow.ViewModels.Pages;
using Hollow.Views;
using Hollow.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hollow;

public partial class App : Application
{
    private static IServiceProvider? _provider;
    public override void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDir, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] (Thread: {ThreadId}) {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
            .CreateLogger();
        
        AvaloniaXamlLoader.Load(this);
        _provider = ConfigureServices();
    }
    
    private static ServiceProvider ConfigureServices()
    {
        var viewLocator = Current?.DataTemplates.First(x => x is ViewLocator);
        var services = new ServiceCollection();

        if (viewLocator is not null)
            services.AddSingleton(viewLocator);
        
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
        
        services.AddSingleton<Screenshots>();
        services.AddSingleton<ScreenshotsViewModel>();
        
        services.AddSingleton<Settings>();
        services.AddSingleton<SettingsViewModel>();

        services.AddSingleton<HttpClient>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMiHoYoLauncherService, MiHoYoLauncherService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IGachaService, GachaService>();

        return services.BuildServiceProvider();
    }
    
    public static T GetService<T>() where T : notnull => _provider!.GetRequiredService<T>();

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = GetService<MainWindow>();
            mainWindow.DataContext = GetService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}