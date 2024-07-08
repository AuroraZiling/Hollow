using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Controls;
using Hollow.Core.Gacha.Uigf;
using Hollow.Enums;
using Hollow.Models;
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
        await Task.Delay(1000);
        GetGachaLogMessage = "Gacha Records";
        if(_gachaService.GachaRecords is null)
        {
            _gachaRecords = await _gachaService.LoadGachaRecords();
        }
        else
        {
            _gachaRecords = _gachaService.GachaRecords;
        }

        Console.WriteLine(_gachaRecords!.Count);
        UidList = new ObservableCollection<string>(_gachaRecords!.Keys);
        SelectedUid = UidList[0];
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