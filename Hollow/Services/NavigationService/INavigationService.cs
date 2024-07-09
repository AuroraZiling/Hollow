using System;

namespace Hollow.Services.NavigationService;

public interface INavigationService
{
    public string CurrentViewName { get; set; }
    public int CurrentViewId { get; set; }

    public Action? CurrentViewChanged { get; set; }

    public void Navigate(int destination);
}