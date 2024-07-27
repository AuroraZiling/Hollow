using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Models.Pages.SignalSearch;

namespace Hollow.Views.Dialogs;

public partial class ImportDialog : UserControl
{
    public ImportDialog(GachaRecords importRecords, Action<ImportItem[]> importCallback)
    {
        InitializeComponent();

        DataContext = new ImportDialogViewModel(importRecords, importCallback);
    }
}