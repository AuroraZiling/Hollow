using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Views.Controls;

namespace Hollow.Views.Dialogs;

public partial class ExportDialogViewModel(
    ObservableCollection<string> uidList,
    Action<string[]> selectedUidListCallback)
    : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> _uidList = uidList;
    
    [ObservableProperty]
    private ObservableCollection<string> _selectedUidList = [];

    [RelayCommand]
    private void Ok()
    {
        selectedUidListCallback(SelectedUidList.ToArray());
        HollowHost.CloseDialog();
    }

    [RelayCommand]
    private void Cancel()
    {
        HollowHost.CloseDialog();
    }
}