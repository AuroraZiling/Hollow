using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Core.Gacha.Uigf;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Models;
using Hollow.Models.Pages.SignalSearch;
using Hollow.Services.GachaService;

namespace Hollow.ViewModels.Pages;

public partial class SignalSearchViewModel : ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
    }

    [ObservableProperty] private string _getGachaLogTitle = "Loading...";
    [ObservableProperty] private string _getGachaLogMessage = "";
    [ObservableProperty] private string _getGachaLogShortMessage = "";
    
    [ObservableProperty] private double _getGachaLogCoverageOpacity;
    [ObservableProperty] private double _gachaTabViewOpacity = 1;
    [ObservableProperty] private bool _controlEnabled = true;
    
    [ObservableProperty] private ObservableCollection<string> _uidList = new();
    [ObservableProperty] private string _selectedUid = "";
    
    private Dictionary<string, GachaRecords>? _gachaRecords;
    private Dictionary<string, AnalyzedGachaRecords>? _analyzedGachaRecords;
    [ObservableProperty] private AnalyzedGachaRecords _selectedAnalyzedGachaRecords;
    
    private readonly IGachaService _gachaService;
    public SignalSearchViewModel(IGachaService gachaService)
    {
        _gachaService = gachaService;

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

    private async Task LoadGachaRecords()
    {
        IntoCoverage();
        
        // Gacha Records
        GetGachaLogShortMessage = "Gacha Records";
        if(_gachaService.GachaRecords is null)
        {
            _gachaRecords = await _gachaService.LoadGachaRecords();
        }
        else
        {
            _gachaRecords = _gachaService.GachaRecords;
        }
        await Task.Delay(100);
        
        // Analyze
        GetGachaLogShortMessage = "Analyzing";
        _analyzedGachaRecords = new Dictionary<string, AnalyzedGachaRecords>();

        foreach (var record in _gachaRecords!)
        {
            var key = record.Key;
            
            var value = record.Value;
            _analyzedGachaRecords.Add(key, GachaAnalyser.FromGachaRecords(value));
        }

        UidList = new ObservableCollection<string>(_gachaRecords!.Keys);
        SelectedUid = UidList[0];
        SelectedAnalyzedGachaRecords = _analyzedGachaRecords[UidList[0]];
        await Task.Delay(100);

        RemoveCoverage();
    }

    [RelayCommand]
    private async Task UpdateByWebCaches()
    {
        var authKey = await _gachaService.TryGetAuthKey();
        if (!authKey.IsSuccess)
        {
            await HollowHost.ShowToast("Failed to get records", authKey.Message, NotificationType.Error);
            return;
        }

        IntoCoverage();
        var gachaProgress = new Progress<Response<string>>(value =>
        {
            var data = value.Data;
            GetGachaLogMessage = data.Replace('^', ' ')[..^1];
            GetGachaLogShortMessage = $"Get {data.Split('^').Length} Items from {data[^1]}";
        });
        var gachaRecord = await _gachaService.TryGetGachaLogs(authKey.Data, gachaProgress);
        await File.WriteAllTextAsync(Path.Combine(AppInfo.GachaRecordsDir, $"{gachaRecord.Data.Info.Uid}.json"), JsonSerializer.Serialize(gachaRecord.Data, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        RemoveCoverage();
    }
}