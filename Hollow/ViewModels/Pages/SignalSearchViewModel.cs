using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Core.Gacha.Uigf;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Models;
using Hollow.Models.Pages.SignalSearch;
using Hollow.Services.GachaService;
using Hollow.Services.NavigationService;

namespace Hollow.ViewModels.Pages;

public partial class SignalSearchViewModel : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
        if (_navigationService.CurrentViewName != "SignalSearch") return;
        
        GetGachaLogTitle = Lang.SignalSearch_ProhibitedCoverage_Loading;
        if (GetGachaLogShortMessage == "Gacha Records Not Found")
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
    
    private Dictionary<string, GachaRecordProfile>? _gachaProfiles;
    private Dictionary<string, AnalyzedGachaRecordProfile>? _analyzedGachaProfiles;
    [ObservableProperty] private AnalyzedGachaRecordProfile? _selectedAnalyzedGachaRecords;
    
    private readonly IGachaService _gachaService;
    private readonly INavigationService _navigationService;
    public SignalSearchViewModel(IGachaService gachaService, INavigationService navigationService)
    {
        _gachaService = gachaService;
        _navigationService = navigationService;

        _navigationService.CurrentViewChanged += Navigated;

        _ = LoadGachaRecords();
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
        await Task.Delay(100);

        RemoveCoverage();
    }

    [RelayCommand]
    private void ChangeUid()
    {
        if (SelectedUid != null && _analyzedGachaProfiles!.TryGetValue(SelectedUid, out var value))
        {
            SelectedAnalyzedGachaRecords = value;
        }
    }

    [RelayCommand]
    private async Task UpdateByWebCaches()
    {
        var authKey = _gachaService.GetAuthKey();
        if (!authKey.IsSuccess)
        {
            await HollowHost.ShowToast(Lang.SignalSearch_Update_GetRecordsFailed, authKey.Message, NotificationType.Error);
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
            else
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
        });
        var gachaRecord = await _gachaService.GetGachaRecords(authKey.Data, gachaProgress);
        
        await File.WriteAllTextAsync(AppInfo.GachaRecordPath, JsonSerializer.Serialize(gachaRecord.Data, HollowJsonSerializer.Options));
        await LoadGachaRecords(uid);
        RemoveCoverage();
    }
}