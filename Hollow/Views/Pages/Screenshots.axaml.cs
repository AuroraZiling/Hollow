using Avalonia.Controls;
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