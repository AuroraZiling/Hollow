using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Enums;
using Hollow.Models.Wiki;
using Hollow.Views.Controls;

namespace Hollow.ViewModels.Pages;

public partial class WikiViewModel : ViewModelBase, IViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<WikiItemModel> _wikiItems = new();

    public WikiViewModel()
    {
        WikiItems.Add(new WikiItemModel
        {
            AvatarUrl = "https://api.hakush.in/zzz/UI/IconRoleSelect01.webp",
            Name = "Abby",
            IsARankType = true
        });
    }
    
    public void Navigated()
    {
        
    }
}