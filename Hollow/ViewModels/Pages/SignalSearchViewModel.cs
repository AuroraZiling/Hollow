using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Gacha;
using Hollow.Abstractions.Models.HttpContrasts.Gacha.Uigf;
using Hollow.Abstractions.Models.SignalSearch;
using Hollow.Abstractions.Services;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Models.SignalSearch;
using Hollow.Views.Controls;
using Hollow.Views.Dialogs;
using Hollow.Views.Pages;
using Serilog;

namespace Hollow.ViewModels.Pages;

public partial class SignalSearchViewModel : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
        if (_navigationService.CurrentViewName != nameof(SignalSearch)) return;
        GetGachaLogTitle = Lang.SignalSearch_ProhibitedCoverage_Loading;

        if(GetGachaLogShortMessage == "")
        {
            _ = LoadGachaRecords();
        }
    }

    [ObservableProperty] private string _getGachaLogTitle = Lang.SignalSearch_ProhibitedCoverage_Loading;
    [ObservableProperty] private string _getGachaLogMessage = "";
    [ObservableProperty] private string _getGachaLogShortMessage = "";
    
    [ObservableProperty] private double _getGachaLogCoverageOpacity;
    [ObservableProperty] private double _gachaTabViewOpacity = 1;
    [ObservableProperty] private bool _controlEnabled = true;
    
    [ObservableProperty] private ObservableCollection<string> _uidList = [];
    [ObservableProperty] private string? _selectedUid;
    
    // Timezone
    [ObservableProperty] private bool _isTimezoneEqual = true;
    [ObservableProperty] private string _displayLocalTimezone;
    [ObservableProperty] private string _selectedUidTimezone;
    
    private Dictionary<string, GachaRecordProfile>? _gachaProfiles;
    private Dictionary<string, AnalyzedGachaRecordProfile>? _analyzedGachaProfiles;
    [ObservableProperty] private AnalyzedGachaRecordProfile? _selectedAnalyzedGachaRecords;
    
    private readonly IGachaService _gachaService;
    private readonly INavigationService _navigationService;
    private readonly IGameService _gameService;
    private readonly IMetadataService _metadataService;
    public SignalSearchViewModel(IGachaService gachaService, INavigationService navigationService, IGameService gameService, IMetadataService metadataService)
    {
        _gachaService = gachaService;
        _navigationService = navigationService;
        _gameService = gameService;
        _metadataService = metadataService;

        var localTimeZoneOffset = TimeZoneAdjuster.LocalTimeZone.BaseUtcOffset.Hours;
        DisplayLocalTimezone = localTimeZoneOffset.ToUtcPrefixTimeZone();
        SelectedUidTimezone = DisplayLocalTimezone;

        _navigationService.CurrentViewChanged += Navigated;
    }

    private void IntoCoverage()
    {
        GetGachaLogCoverageOpacity = 1;
        GachaTabViewOpacity = 0;
        ControlEnabled = false;
    }
    
    private void RemoveCoverage()
    {
        GetGachaLogCoverageOpacity = 0;
        GachaTabViewOpacity = 1;
        ControlEnabled = true;
    }

    private async Task LoadGachaRecords(string? updatedUid = null)
    {
        IntoCoverage();
        
        // Gacha Records
        GetGachaLogShortMessage = Lang.SignalSearch_LoadGachaRecords_GachaRecords;
        _gachaProfiles = await _gachaService.LoadGachaRecordProfileDictionary();
        if (_gachaProfiles is null || _gachaProfiles!.Count == 0)
        {
            UidList = [];
            SelectedUid = null;
            SelectedAnalyzedGachaRecords = null;
            
            GetGachaLogShortMessage = Lang.SignalSearch_LoadGachaRecords_GachaRecordsNotFound;
            ControlEnabled = true;
            return;
        }
        
        // Analyze
        GetGachaLogShortMessage = Lang.SignalSearch_LoadGachaRecords_Analyze;
        _analyzedGachaProfiles = GachaAnalyser.FromGachaProfiles(_gachaProfiles);

        UidList = new ObservableCollection<string>(_analyzedGachaProfiles!.Keys);
        SelectedUid = UidList.FirstOrDefault(uid => uid == updatedUid) ?? UidList.First();
        SelectedAnalyzedGachaRecords = _analyzedGachaProfiles[SelectedUid];

        await Task.Delay(200);

        RemoveCoverage();
    }

    [RelayCommand]
    private void DeleteProfile()
    {
        HollowHost.ShowDialog(new DeleteProfileDialog(SelectedUid!, UidDeleteConfirmCallback));
        return;

        async void UidDeleteConfirmCallback(bool confirmed)
        {
            if (!confirmed) return;
            var gachaRecords = new GachaRecords { Info = { ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }, Profiles = UidList.Where(uid => uid != SelectedUid).Select(uid => _gachaProfiles![uid]).ToList() };
            await File.WriteAllTextAsync(AppInfo.GachaRecordPath, JsonSerializer.Serialize(gachaRecords, HollowJsonSerializer.Options));
            await LoadGachaRecords();
            Log.Information("[SignalSearch] Profile deleted: {Uid}", SelectedUid);
        }
    }

    [RelayCommand]
    private void ExportRecords()
    {
        HollowHost.ShowDialog(new ExportDialog(UidList, SelectedUidListCallback));
        return;

        async void SelectedUidListCallback(string[] selectedUidList)
        {
            if (selectedUidList.Length == 0) return;
            var gachaRecords = new GachaRecords
            {
                Info =
                {
                    ExportApp = "Hollow",
                    ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                }, 
                Profiles = selectedUidList.Select(uid => _gachaProfiles![uid]).ToList()
            };

            var file = await App.MainWindowInstance.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions { SuggestedFileName = $"Hollow_{gachaRecords.Info.ExportTimestamp}.json"});
            if (file is null) return;
            await using var stream = await file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(gachaRecords, HollowJsonSerializer.Options));
            Log.Information("[SignalSearch] Records exported: {UidList} to {File}", selectedUidList, file.Path);
        }
    }
    
    [RelayCommand]
    private void ChangeUid()
    {
        if (SelectedUid != null && _analyzedGachaProfiles!.TryGetValue(SelectedUid, out var value))
        {
            SelectedAnalyzedGachaRecords = value;
            IsTimezoneEqual = value.DisplayTimezone == DisplayLocalTimezone;
            SelectedUidTimezone = SelectedAnalyzedGachaRecords.DisplayTimezone;
            App.MainWindowInstance.UpdateLayout();
        }
    }

    [RelayCommand]
    private async Task UpdateByImport()
    {
        if(_metadataService.ItemsMetadata is null)
        {
            HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_MetadataNotFound_Message, NotificationType.Error);
        }
        
        var files = await App.MainWindowInstance.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Lang.SignalSearch_Import_FilePickerTitle,
            FileTypeFilter = new[] { new("UIGF v4.0") { Patterns = new[] { "*.json" } }, FilePickerFileTypes.All },
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var fileContent = await streamReader.ReadToEndAsync();
            if (!UigfSchemaValidator.Validate(fileContent))
            {
                HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_InvalidUigfFile_Message, NotificationType.Error);
                return;
            }
            
            var fileJson = JsonSerializer.Deserialize<GachaRecords>(fileContent, HollowJsonSerializer.Options);
            if (fileJson is null)
            {
                HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_InvalidUigfFile_Message, NotificationType.Error);
                return;
            }
            
            if(fileJson.Profiles.Count == 0)
            {
                HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_EmptyUigfNapRecords_Message, NotificationType.Error);
                return;
            }
            
            HollowHost.ShowDialog(new ImportDialog(fileJson, SelectedImportItemsCallback));
            return;

            async void SelectedImportItemsCallback(ImportItem[] selectedImportItems)
            {
                if (selectedImportItems.Length == 0) return;
                var gachaRecords = _gachaService.MergeGachaRecordsFromImport(fileJson, selectedImportItems, _metadataService.ItemsMetadata!);
                await File.WriteAllTextAsync(AppInfo.GachaRecordPath, JsonSerializer.Serialize(gachaRecords, HollowJsonSerializer.Options));
                
                HollowHost.ShowToast(Lang.Toast_Common_Success_Title, string.Format(Lang.Toast_ImportSuccess_Message, fileJson.Info.ExportApp, selectedImportItems.Length), NotificationType.Success);
                await LoadGachaRecords();
            }
        }
    }

    [RelayCommand]
    private void UpdateByUrl()
    {
        HollowHost.ShowDialog(new RecordUrlDialog(UrlCallback));
        return;

        async void UrlCallback(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            var authKey = _gachaService.GetGachaUrlDataFromUrl(url);
            
            if (!authKey.IsSuccess)
            {
                HollowHost.ShowToast(Lang.SignalSearch_Update_GetRecordsFailed, authKey.Message, NotificationType.Error);
                return;
            }
            await UpdateRecords(authKey.Data);
        }
    }

    [RelayCommand]
    private async Task UpdateByWebCaches()
    {
        var authKey = _gachaService.GetGachaUrlData();
        if (!authKey.IsSuccess)
        {
            HollowHost.ShowToast(Lang.SignalSearch_Update_GetRecordsFailed, authKey.Message, NotificationType.Error);
            return;
        }
        await UpdateRecords(authKey.Data);
    }

    private async Task UpdateRecords(GachaUrlData gachaUrlData)
    {
        if (!await _gachaService.IsAuthKeyValid(gachaUrlData.AuthKey))
        {
            HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_InvalidAuthKey_Message, NotificationType.Error);
            return;
        }
        
        IntoCoverage();

        string? uid = null;
        var gachaProgress = new Progress<Response<string>>(value =>
        {
            if (value.Message.StartsWith("success"))
            {
                var message = value.Message.Split(' ');
                HollowHost.ShowToast(Lang.SignalSearch_Update_Success, string.Format(Lang.SignalSearch_Update_SuccessMessage, message[1], message[2]),
                    NotificationType.Success);
            }
            else if(value.IsSuccess)
            {
                var data = value.Data.Split('^');
                var gachaType = data[^2] switch
                {
                    "1" => Lang.SignalSearch_StandardChannel,
                    "2" => Lang.SignalSearch_ExclusiveChannel,
                    "3" => Lang.SignalSearch_WEngineChannel,
                    "5" => Lang.SignalSearch_BangbooChannel,
                    _ => Lang.SignalSearch_UnknownChannel
                };
                GetGachaLogMessage = string.Join(' ', data[..^3]);
                GetGachaLogTitle = string.Format(Lang.SignalSearch_Update_ProgressTitle, gachaType);
                GetGachaLogShortMessage = $"UID {data[^3]} | {string.Format(Lang.SignalSearch_Update_ProgressMessage, data.Length-3, data[^1])}";
                uid ??= data[^3];
            }
            else
            {
                HollowHost.ShowToast(Lang.Toast_Common_Error_Title, value.Message, NotificationType.Error);
            }
        });
        GetGachaLogMessage = "";
        var gachaRecord = await _gachaService.GetGachaRecords(gachaUrlData, gachaProgress, _gameService.GameBiz);
        if (gachaRecord.IsSuccess)
        {
            await File.WriteAllTextAsync(AppInfo.GachaRecordPath, JsonSerializer.Serialize(gachaRecord.Data, HollowJsonSerializer.Options));
            await LoadGachaRecords(uid);
            RemoveCoverage();
        }

        ControlEnabled = true;
    }
}