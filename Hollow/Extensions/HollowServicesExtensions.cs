using System.Net.Http;
using Hollow.Abstractions.Services;
using Hollow.Services;
using Hollow.ViewModels.Pages;
using Hollow.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Hollow.Extensions;

public static class HollowServicesExtensions
{
    public static void AddHollowServices(this IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMiHoYoLauncherService, MiHoYoLauncherService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IGachaService, GachaService>();
        services.AddSingleton<IMetadataService, MetadataService>();
    }
    
    public static void AddHollowViews(this IServiceCollection services)
    {
        services.AddSingleton<Home>();
        services.AddSingleton<Announcements>();
        services.AddSingleton<GameSettings>();
        services.AddSingleton<SignalSearch>();
        services.AddSingleton<Wiki>();
        services.AddSingleton<Settings>();
    }

    public static void AddHollowViewModels(this IServiceCollection services)
    {
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<AnnouncementsViewModel>();
        services.AddSingleton<GameSettingsViewModel>();
        services.AddSingleton<SignalSearchViewModel>();
        services.AddSingleton<WikiViewModel>();
        services.AddSingleton<SettingsViewModel>();
    }
}