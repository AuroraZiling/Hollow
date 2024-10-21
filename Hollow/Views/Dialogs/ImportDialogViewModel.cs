using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.SignalSearch;
using Hollow.Helpers;
using Hollow.Views.Controls;

namespace Hollow.Views.Dialogs;

public partial class ImportDialogViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ImportItem> _importItems;
    [ObservableProperty] private ObservableCollection<ImportItem> _selectedImportItems = [];
    
    [ObservableProperty] private string _recordFormat;
    [ObservableProperty] private string _exportApplication;
    [ObservableProperty] private string _exportTime;

    private readonly Action<ImportItem[]> _importCallback;

    public ImportDialogViewModel(GachaRecords importRecords, Action<ImportItem[]> importCallback)
    {
        RecordFormat = $"UIGF {importRecords.Info.UigfVersion}";
        ExportApplication = $"{importRecords.Info.ExportApp} ({importRecords.Info.ExportAppVersion})";
        ExportTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(importRecords.Info.ExportTimestamp)).LocalDateTime
            .ToString("yyyy-MM-dd HH:mm:ss");
        ImportItems = new ObservableCollection<ImportItem>(importRecords.Profiles.Select(profile => new ImportItem
        {
            Uid = profile.Uid,
            I18NTimezone = profile.Timezone.ToUtcPrefixTimeZone(),
            IsChinaServer = profile.Uid.Length == 8
        }));
        
        _importCallback = importCallback;
    }

    [RelayCommand]
    private void Ok()
    {
        _importCallback(SelectedImportItems.ToArray());
        HollowHost.CloseDialog();
    }

    [RelayCommand]
    private void Cancel()
    {
        HollowHost.CloseDialog();
    }
}