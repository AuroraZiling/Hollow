using System;
using Avalonia.Controls;
using Hollow.ViewModels;
using Hollow.Views.Pages;

namespace Hollow.Services.NavigationService;

public class NavigationService: INavigationService
{
    public UserControl CurrentView { get; set; } = App.GetService<Home>();
    public string CurrentViewName { get; set; } = "Home";
    
    public Action? CurrentViewChanged { get; set; }
    
    public void Navigate(string destination)
    {
        CurrentView = destination switch
        {
            "Home" => App.GetPage<Home>(),
            "GameSettings" => App.GetPage<GameSettings>(),
            "SignalSearch" => App.GetPage<SignalSearch>(),
            "Screenshots" => App.GetPage<Screenshots>(),
            "Settings" => App.GetPage<Settings>(),
            _ => throw new ArgumentException("Invalid destination")
        };
        CurrentViewName = destination;
        CurrentViewChanged?.Invoke();
    }
}