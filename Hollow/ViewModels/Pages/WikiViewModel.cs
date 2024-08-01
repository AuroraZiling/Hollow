using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Abstractions.Enums.Hakush;
using Hollow.Enums;
using Hollow.Helpers;
using Hollow.Languages;
using Hollow.Models.Wiki;
using Hollow.Services.MetadataService;
using Hollow.Services.NavigationService;
using Hollow.Views.Controls;
using Hollow.Views.Pages;

namespace Hollow.ViewModels.Pages;

public partial class WikiViewModel : ViewModelBase, IViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<WikiCharacterItemModel> _wikiCharacterItems = new();
    [ObservableProperty]
    private ObservableCollection<WikiWeaponItemModel> _wikiWeaponItems = new();
    [ObservableProperty]
    private ObservableCollection<WikiBangbooItemModel> _wikiBangbooItems = new();
    [ObservableProperty]
    private ObservableCollection<WikiEquipmentItemModel> _wikiEquipmentItems = new();

    private readonly IMetadataService _metadataService;
    private readonly INavigationService _navigationService;
    public WikiViewModel(IMetadataService metadataService, INavigationService navigationService)
    {
        _metadataService = metadataService;
        _navigationService = navigationService;
        
        _navigationService.CurrentViewChanged += Navigated;
    }

    private void LoadWiki()
    {
        foreach (var itemPair in _metadataService.ItemsMetadata!)
        {
            if (itemPair.Value is { ItemType: HakushItemType.Character, IsCompleted: true })
            {
                WikiCharacterItems.Add(new WikiCharacterItemModel
                {
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
        LoadWiki();
    }
}