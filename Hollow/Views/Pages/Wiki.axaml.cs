using Avalonia.Controls;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Wiki : UserControl
{
    public Wiki()
    {
        InitializeComponent();
        DataContext = App.GetService<WikiViewModel>();
    }
}