using System;
using Avalonia.Controls;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.SignalSearch;
using Hollow.Models.SignalSearch;

namespace Hollow.Views.Dialogs;

public partial class ImportDialog : UserControl
{
    public ImportDialog(GachaRecords importRecords, Action<ImportItem[]> importCallback)
    {
        InitializeComponent();

        DataContext = new ImportDialogViewModel(importRecords, importCallback);
    }
}