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
            "Home" => App.GetService<Home>(),
            "GameSettings" => App.GetService<GameSettings>(),
            "SignalSearch" => App.GetService<SignalSearch>(),
            "Screenshots" => App.GetService<Screenshots>(),
            "Settings" => App.GetService<Settings>(),
            _ => throw new ArgumentException("Invalid destination")
        };
        CurrentViewName = destination;
        CurrentViewChanged?.Invoke();
    }
}