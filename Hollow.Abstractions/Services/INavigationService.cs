namespace Hollow.Abstractions.Services;

public interface INavigationService
{
    public string CurrentViewName { get; set; }
    public int CurrentViewId { get; set; }

    public Action? CurrentViewChanged { get; set; }

    public void Navigate(int destination);
}