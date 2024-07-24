using Avalonia.Controls;
using Avalonia.Data;
using Hollow.Helpers.Converters;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class SignalSearch : UserControl
{
    public SignalSearch()
    {
        InitializeComponent();
        DataContext = App.GetService<SignalSearchViewModel>();
    }

    private void DataGrid_OnLoadingRow(object? _, DataGridRowEventArgs e)
    {
        var row = e.Row;
        row.Bind(ForegroundProperty, 
            new Binding("RankType", BindingMode.OneWay) { Converter = new RankTypeToBrushConverter() });
    }
}