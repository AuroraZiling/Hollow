using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace Hollow.Views.Dialogs;

public partial class ExportDialog : UserControl
{
    public ExportDialog(ObservableCollection<string> uidList, Action<string[]> selectedUidListCallback)
    {
        InitializeComponent();

        DataContext = new ExportDialogViewModel(uidList, selectedUidListCallback);
    }
}