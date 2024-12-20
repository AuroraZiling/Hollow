﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollow.Abstractions.Enums.Hakush;
using Hollow.Abstractions.JsonConverters.Serializers;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;
using Hollow.Abstractions.Services;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Models.Wiki;
using Hollow.Services;
using Hollow.Views.Controls;
using Hollow.Views.Pages;

namespace Hollow.ViewModels.Pages;

public partial class WikiViewModel : ViewModelBase, IViewModelBase
{
    #region Loading Coverage
    
    [ObservableProperty] private double _loadingCoverageOpacity;
    [ObservableProperty] private bool _loadingCoverageVisible;
    [ObservableProperty] private string _loadingCoverageMessage = Lang.Wiki_LoadingCoverage_DefaultMessage;
    
    private void ShowCoverage(string message)
    {
        LoadingCoverageMessage = message;
        LoadingCoverageOpacity = 1;
        LoadingCoverageVisible = true;
    }
    
    private void HideCoverage()
    {
        LoadingCoverageMessage = "";
        LoadingCoverageOpacity = 0;
        LoadingCoverageVisible = false;
    }
    #endregion
    
    #region Character
    
    private const string CharacterDetailApiUrl = "https://api.hakush.in/zzz/data/zh/character";
    
    [ObservableProperty] private ObservableCollection<WikiCharacterItemModel> _wikiCharacterItems = [];
    [ObservableProperty] private WikiCharacterItemModel? _selectedCharacterItem;
    [ObservableProperty] private HakushCharacterModel? _selectedCharacterDetailItem;
    [ObservableProperty] private bool _isCharacterItemLoading;

    partial void OnSelectedCharacterItemChanged(WikiCharacterItemModel? value)
    {
        if (value is null) return;
        Task.Run(async () =>
        {
            ShowCoverage(string.Format(Lang.Wiki_LoadingCoverage_FetchDataMessage, value.Name));
            SelectedCharacterDetailItem = await LoadCharacterInfo(value.Id);
            if (SelectedCharacterDetailItem is null)
            {
                HollowHost.ShowToast(Lang.Toast_Common_Error_Title, string.Format(Lang.Toast_WikiItemLoadFailed_Message, value.Name), NotificationType.Error);
            }
            while (IsCharacterItemLoading) await Task.Delay(200);
            HideCoverage();
        });
    }

    private async Task<HakushCharacterModel?> LoadCharacterInfo(string selectedCharacterId)
    {
        var response = await _httpClient.GetStringAsync($"{CharacterDetailApiUrl}/{selectedCharacterId}.json");
        var data = JsonSerializer.Deserialize<HakushCharacterModel>(response, HollowJsonSerializer.Options);
        if (data is null) return null;
        data.Icon = $"{MetadataService.ItemMetadataIconBaseUrl}/{data.Icon}.webp";
        return data;
    }
    
    #endregion
    
    
    [ObservableProperty]
    private ObservableCollection<WikiWeaponItemModel> _wikiWeaponItems = [];
    [ObservableProperty]
    private WikiWeaponItemModel? _selectedWeaponItem;
    
    [ObservableProperty]
    private ObservableCollection<WikiBangbooItemModel> _wikiBangbooItems = [];
    [ObservableProperty]
    private WikiBangbooItemModel? _selectedBangbooItem;
    
    [ObservableProperty]
    private ObservableCollection<WikiEquipmentItemModel> _wikiEquipmentItems = [];
    [ObservableProperty]
    private WikiEquipmentItemModel? _selectedEquipmentItem;

    private readonly IMetadataService _metadataService;
    private readonly INavigationService _navigationService;
    private readonly HttpClient _httpClient;
    public WikiViewModel(IMetadataService metadataService, INavigationService navigationService, HttpClient httpClient)
    {
        _metadataService = metadataService;
        _navigationService = navigationService;
        _httpClient = httpClient;

        _navigationService.CurrentViewChanged += Navigated;
    }

    private void LoadWiki()
    {
        WikiCharacterItems.Clear();
        WikiWeaponItems.Clear();
        WikiBangbooItems.Clear();
        WikiEquipmentItems.Clear();
        foreach (var itemPair in _metadataService.ItemsMetadata!)
        {
            if (itemPair.Value is { ItemType: HakushItemType.Character, IsCompleted: true })
            {
                WikiCharacterItems.Add(new WikiCharacterItemModel
                {
                    Id = itemPair.Key,
                    AvatarUrl = itemPair.Value.Icon,
                    Name = itemPair.Value.ChineseName,
                    TypeIconResBitmap = ImageHelper.LoadFromResource(new Uri(itemPair.Value.TypeIconRes)),
                    ElementIconResBitmap = ImageHelper.LoadFromResource(new Uri(itemPair.Value.CharacterElementIconRes)),
                    IsARankType = itemPair.Value.RankType == 3,
                    IsSRankType = itemPair.Value.RankType == 4
                });
            }
            else if (itemPair.Value.ItemType == HakushItemType.Weapon)
            {
                WikiWeaponItems.Add(new WikiWeaponItemModel
                {
                    Id = itemPair.Key,
                    AvatarUrl = itemPair.Value.Icon,
                    Name = itemPair.Value.ChineseName,
                    TypeIconResBitmap = ImageHelper.LoadFromResource(new Uri(itemPair.Value.TypeIconRes)),
                    IsBRankType = itemPair.Value.RankType == 2,
                    IsARankType = itemPair.Value.RankType == 3,
                    IsSRankType = itemPair.Value.RankType == 4
                });
            }
            else if (itemPair.Value.ItemType == HakushItemType.Bangboo)
            {
                WikiBangbooItems.Add(new WikiBangbooItemModel
                {
                    Id = itemPair.Key,
                    AvatarUrl = itemPair.Value.Icon,
                    Name = itemPair.Value.ChineseName,
                    IsBRankType = itemPair.Value.RankType == 2,
                    IsARankType = itemPair.Value.RankType == 3,
                    IsSRankType = itemPair.Value.RankType == 4
                });
            }
            else if (itemPair.Value.ItemType == HakushItemType.Equipment)
            {
                WikiEquipmentItems.Add(new WikiEquipmentItemModel
                {
                    Id = itemPair.Key,
                    AvatarUrl = itemPair.Value.Icon,
                    Name = itemPair.Value.ChineseName
                });
            }
        }

        WikiCharacterItems = new ObservableCollection<WikiCharacterItemModel>(WikiCharacterItems.OrderByDescending(item => item.IsSRankType));
        WikiWeaponItems = new ObservableCollection<WikiWeaponItemModel>(WikiWeaponItems.OrderByDescending(item => item.IsSRankType).ThenByDescending(item => item.IsARankType));
        WikiBangbooItems = new ObservableCollection<WikiBangbooItemModel>(WikiBangbooItems.OrderByDescending(item => item.IsSRankType).ThenByDescending(item => item.IsARankType));
    }
    
    public void Navigated()
    {
        if (_navigationService.CurrentViewName != nameof(Wiki)) return;
        if(_metadataService.ItemsMetadata is null)
        {
            HollowHost.ShowToast(Lang.Toast_Common_Error_Title, Lang.Toast_MetadataNotFound_Message, NotificationType.Error);
            return;
        }

        LoadingCoverageMessage = Lang.Wiki_LoadingCoverage_DefaultMessage;
        LoadWiki();
    }
}