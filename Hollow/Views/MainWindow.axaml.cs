using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Hollow.Abstractions.Services;
using Serilog;

namespace Hollow.Views;

public partial class MainWindow : Window
{
    private INavigationService? _navigationService;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MinimizeButton_OnClick(object? _1, RoutedEventArgs _2)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_OnClick(object? _1, RoutedEventArgs _2)
    {
        Log.CloseAndFlush();
        Environment.Exit(0);
        Log.Information("[App] Hollow is shutting down");
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if(e.GetCurrentPoint(this).Position.Y <= 40)
            BeginMoveDrag(e);
    }

    private void NavigationTabControl_OnSelectionChanged(object? _, SelectionChangedEventArgs e)
    {
        if (NavigationTabControl != null && e.Source is TabControl { Name: "NavigationTabControl" })
        {
            _navigationService!.Navigate(NavigationTabControl.SelectedIndex);
        }
        else if (NavigationTabControl == null)
        {
            _navigationService = App.GetService<INavigationService>();
        }
    }
}