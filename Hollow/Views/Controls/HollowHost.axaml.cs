using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Hollow.Helpers;

namespace Hollow.Views.Controls;


public class HollowHost : ContentControl
{
    // Dialogs
    public static readonly StyledProperty<bool> IsDialogOpenProperty =
        AvaloniaProperty.Register<HollowHost, bool>(nameof(IsDialogOpen), defaultValue: false);

    public bool IsDialogOpen
    {
        get => GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    public static readonly StyledProperty<Control> DialogContentProperty =
        AvaloniaProperty.Register<HollowHost, Control>(nameof(DialogContent));

    public Control DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    public static readonly StyledProperty<bool> AllowBackgroundCloseProperty =
        AvaloniaProperty.Register<HollowHost, bool>(nameof(AllowBackgroundClose), defaultValue: true);

    public bool AllowBackgroundClose
    {
        get => GetValue(AllowBackgroundCloseProperty);
        set => SetValue(AllowBackgroundCloseProperty, value);
    }
    
    public static void ShowDialog(Window window, object? content, bool showCardBehind = true,
        bool allowBackgroundClose = false)
    {
        if (!Instances.TryGetValue(window, out var host))
            throw new InvalidOperationException("No HollowHost present in this window");
        var control = content as Control ?? DialogViewLocator.TryBuild(content);
        host.IsDialogOpen = true;
        host.DialogContent = control;
        host.AllowBackgroundClose = allowBackgroundClose;
        host.GetTemplateChildren().First(n => n.Name == "InnerBorderDialog").Opacity = showCardBehind ? 1 : 0;
    }
    
    public static void ShowDialog(object? content, bool showCardBehind = true, bool allowBackgroundClose = false)
    {
        if (_mainWindow != null) ShowDialog(_mainWindow, content, showCardBehind, allowBackgroundClose);
    }

    public static void CloseDialog(Window window)
    {
        if (!Instances.TryGetValue(window, out var host))
            throw new InvalidOperationException("No HollowHost present in this window");
        host.IsDialogOpen = false;
    }
    
    private static void BackgroundRequestClose(HollowHost host)
    {
        if (!host.AllowBackgroundClose) return;
        host.IsDialogOpen = false;
    }
    
    public static void CloseDialog()
    {
        if (_mainWindow != null) CloseDialog(_mainWindow);
    }
    
    // Common
    private static Window? _mainWindow;
    private static readonly Dictionary<Window, HollowHost> Instances = new();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (VisualRoot is not Window w) return;
        Instances.Add(w, this);
        _mainWindow ??= w;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (VisualRoot is not Window)
            throw new InvalidOperationException("HollowHost must be hosted inside a Window");
        
        e.NameScope.Get<Border>("PART_DialogBackground").PointerPressed += (_, _) => BackgroundRequestClose(this);
        
        var b = e.NameScope.Get<Border>("PART_DialogBackground");
        b.Loaded += (_, _) => ControlAnimationHelper.MakeOpacityAnimate(ElementComposition.GetElementVisual(b)!, 400); 
    }

    public static WindowNotificationManager NotificationManager { get; set; } = null!;
    public static void ShowToast(string title, string message, NotificationType notificationType, TimeSpan? timeSpan = null, Action? onClick = null, Action? onClose = null)
    {
        NotificationManager.Show(new Notification(title, message, notificationType, timeSpan, onClick, onClose));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (VisualRoot is not Window w) return;
        Instances.Remove(w);
        _mainWindow = Instances.FirstOrDefault().Key;
    }
}