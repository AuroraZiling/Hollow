using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollow.Views.Controls;

namespace Hollow.Views.Dialogs;

public partial class RecordUrlDialogViewModel(Action<string> urlCallback) : ObservableObject
{
    [ObservableProperty]
    private string _url = "";

    [RelayCommand]
    private void Ok()
    {
        urlCallback(Url);
        HollowHost.CloseDialog();
    }

    [RelayCommand]
    private void Cancel()
    {
        urlCallback("");
        HollowHost.CloseDialog();
    }
}