using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Views.Controls;

namespace Hollow.Views.Dialogs;

public partial class DeleteProfileDialogViewModel(string uid, Action<bool> uidDeleteConfirmCallback) : ObservableObject
{
    [ObservableProperty]
    private string _uid = uid;

    [RelayCommand]
    private void Ok()
    {
        uidDeleteConfirmCallback(true);
        HollowHost.CloseDialog();
    }

    [RelayCommand]
    private void Cancel()
    {
        uidDeleteConfirmCallback(false);
        HollowHost.CloseDialog();
    }
}