using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Screenshots : UserControl
{
    public Screenshots()
    {
        InitializeComponent();
        DataContext = App.GetService<ScreenshotsViewModel>();
    }
}