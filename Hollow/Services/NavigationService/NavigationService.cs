using System;

namespace Hollow.Services.NavigationService;

public class NavigationService: INavigationService
{
    public string CurrentViewName { get; set; } = "Home";
    
    public Action? CurrentViewChanged { get; set; }
    
    public void Navigate(int destination)
    {
        CurrentViewName = destination switch
        {
            0 => "Home",
            1 => "Announcements",
            2 => "GameSettings",
            3 => "SignalSearch",
            4 => "Screenshots",
            5 => "Settings",
            _ => throw new ArgumentException("Invalid destination")
        };
        CurrentViewChanged?.Invoke();
    }
}