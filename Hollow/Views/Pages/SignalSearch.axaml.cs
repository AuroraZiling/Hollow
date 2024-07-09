using Avalonia.Controls;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class SignalSearch : UserControl
{
    public SignalSearch()
    {
        InitializeComponent();
        DataContext = App.GetService<SignalSearchViewModel>();
    }
}