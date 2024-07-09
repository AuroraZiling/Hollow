using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Hollow.Views;

public partial class MainWindow : Window
{
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
        Close();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if(e.GetCurrentPoint(this).Position.Y <= 40)
            BeginMoveDrag(e);
    }
}