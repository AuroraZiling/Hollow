using System;
using Hollow.Views.Pages;

namespace Hollow.Services.NavigationService;

public class NavigationService: INavigationService
{
    public string CurrentViewName { get; set; } = "Home";
    public int CurrentViewId { get; set; }
    
    public Action? CurrentViewChanged { get; set; }
    
    public void Navigate(int destination)
    {
        CurrentViewId = destination;
        CurrentViewName = destination switch
        {
            0 => nameof(Home),
            1 => nameof(Announcements),
            2 => nameof(GameSettings),
            3 => nameof(SignalSearch),
            4 => nameof(Wiki),
            5 => nameof(Settings),
            _ => throw new ArgumentException("Invalid destination")
        };
        CurrentViewChanged?.Invoke();
    }
}