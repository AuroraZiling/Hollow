using Avalonia.Controls;
using Avalonia.Interactivity;
using Hollow.Views.Controls;

namespace Hollow.Views.Dialogs;

public partial class StandardDialog : UserControl
{
    public StandardDialog()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        HollowHost.CloseDialog();
    }
}