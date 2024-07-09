using System;
using Avalonia.Controls;

namespace Hollow.Services.NavigationService;

public interface INavigationService
{
    public string CurrentViewName { get; set; }
    public Action? CurrentViewChanged { get; set; }

    public void Navigate(int destination);
}