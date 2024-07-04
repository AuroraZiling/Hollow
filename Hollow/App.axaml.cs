using System;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hollow.Services.ConfigurationService;
using Hollow.Services.MiHoYoLauncherService;
using Hollow.Services.NavigationService;
using Hollow.ViewModels;
using Hollow.ViewModels.Pages;
using Hollow.Views;
using Hollow.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Hollow;

public partial class App : Application
{
    private static IServiceProvider? _provider;
    public override void Initialize()
    {
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