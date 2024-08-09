using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Hollow.Helpers;
using Hollow.Views.Controls.Toast;

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


    // Toasts
    public static readonly AttachedProperty<int> ToastLimitProperty =
        AvaloniaProperty.RegisterAttached<HollowHost, Window, int>("ToastLimit", defaultValue: 5);

    public static int GetToastLimit(Control element) => element.GetValue(ToastLimitProperty);

    public static void SetToastLimit(Control element, int value) =>
        element.SetValue(ToastLimitProperty, value);

    public static readonly StyledProperty<AvaloniaList<Toast.Toast>?> ToastsCollectionProperty =
        AvaloniaProperty.Register<HollowHost, AvaloniaList<Toast.Toast>?>(nameof(ToastsCollection));

    public AvaloniaList<Toast.Toast>? ToastsCollection
    {
        get => GetValue(ToastsCollectionProperty);
        set => SetValue(ToastsCollectionProperty, value);
    }

    private int _maxToasts;
    
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
        if (VisualRoot is not Window window)
            throw new InvalidOperationException("HollowHost must be hosted inside a Window");
        ToastsCollection ??= [];
        _maxToasts = GetToastLimit(window);
        
        e.NameScope.Get<Border>("PART_DialogBackground").PointerPressed += (_, _) => BackgroundRequestClose(this);
        
        var b = e.NameScope.Get<Border>("PART_DialogBackground");
        b.Loaded += (_, _) => ControlAnimationHelper.MakeOpacityAnimate(ElementComposition.GetElementVisual(b)!, 400); 
    }
    
    public static async Task ShowToast(Window window, ToastModel model)
    {
        try
        {
            if (!Instances.TryGetValue(window, out var host))
                throw new InvalidOperationException("No HollowHost present in this window");

            var toast = ToastPool.Get();
            toast.Initialize(model, host);
            if (host.ToastsCollection!.Count >= host._maxToasts)
                await ClearToast(host.ToastsCollection.First());
            Dispatcher.UIThread.Invoke(() =>
            {
                host.ToastsCollection.Add(toast);
                toast.Animate(OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(500));
                toast.Animate(MarginProperty, new Thickness(0, 10, 0, -10), new Thickness(),
                    TimeSpan.FromMilliseconds(500));
            });
        }
        catch
        {
            // ignored
        }
    }
    public static Task ShowToast(ToastModel model) => 
        ShowToast(_mainWindow!, model);

    public static Task ShowToast(string title, string? content, NotificationType? type, TimeSpan? duration = null, Action? onClicked = null) =>
        ShowToast(new ToastModel(
            title,
            content ?? "",
            type ?? NotificationType.Information,
            duration ?? TimeSpan.FromSeconds(4),
            onClicked));

    public static Task ShowToast(Window window, string title, string? content, NotificationType? type, TimeSpan? duration = null,
        Action? onClicked = null) =>
        ShowToast(window, new ToastModel(
            title,
            content ?? "",
            type ?? NotificationType.Information,
            duration ?? TimeSpan.FromSeconds(4),
            onClicked));

    public static async Task ClearToast(Toast.Toast toast)
    {
        var wasRemoved = await Task.Run(async () =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                toast.Animate(OpacityProperty, 1d, 0d, TimeSpan.FromMilliseconds(300));
                toast.Animate(MarginProperty, new Thickness(), new Thickness(0, 20, 0, -20),
                    TimeSpan.FromMilliseconds(300));
            });
            await Task.Delay(300);
            return Dispatcher.UIThread.Invoke(() => toast.Host.ToastsCollection!.Remove(toast));
        });

        if (!wasRemoved) return;
        ToastPool.Return(toast);
    }

    public static void ClearAllToasts(Window window)
    {
        if (!Instances.TryGetValue(window, out var host))
            throw new InvalidOperationException("No HollowHost present in this window");
        ToastPool.Return(host.ToastsCollection!);
        Dispatcher.UIThread.Invoke(() => host.ToastsCollection!.Clear());
    }

    public static void ClearAllToasts() => ClearAllToasts(_mainWindow!);

    #region New Toasts

    public static WindowNotificationManager NotificationManager { get; set; } = null!;
    public static void ShowAvaloniaToast(string title, string message, NotificationType notificationType, TimeSpan? timeSpan = null, Action? onClick = null, Action? onClose = null)
    {
        NotificationManager.Show(new Notification(title, message, notificationType, timeSpan, onClick, onClose));
    }

    #endregion

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (VisualRoot is not Window w) return;
        Instances.Remove(w);
        _mainWindow = Instances.FirstOrDefault().Key;
    }
}