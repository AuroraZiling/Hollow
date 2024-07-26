using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace Hollow.Views.Dialogs;

public partial class RecordUrlDialog : UserControl
{
    public RecordUrlDialog(Action<string> urlCallback)
    {
        InitializeComponent();

        DataContext = new RecordUrlDialogViewModel(urlCallback);
    }
}