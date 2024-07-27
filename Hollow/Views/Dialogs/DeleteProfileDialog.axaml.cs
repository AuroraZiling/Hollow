using System;
using Avalonia.Controls;

namespace Hollow.Views.Dialogs;

public partial class DeleteProfileDialog : UserControl
{
    public DeleteProfileDialog(string uid, Action<bool> uidDeleteConfirmCallback)
    {
        InitializeComponent();

        DataContext = new DeleteProfileDialogViewModel(uid, uidDeleteConfirmCallback);
    }
}