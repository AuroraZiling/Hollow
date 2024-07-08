using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Hollow.Controls.Toast;
using Hollow.Helpers;
using static Hollow.Helpers.ViewLocator;
using NotificationType = Hollow.Enums.NotificationType;

namespace Hollow.Controls;


public class HollowHost : ContentControl
{

    public static readonly AttachedProperty<ToastLocation> ToastLocationProperty =
        AvaloniaProperty.RegisterAttached<HollowHost, Window, ToastLocation>("ToastLocation",
            defaultValue: ToastLocation.BottomRight);

    public static void SetToastLocation(Control element, ToastLocation value) =>
        element.SetValue(ToastLocationProperty, value);

    public static ToastLocation GetToastLocation(Control element) =>
        element.GetValue(ToastLocationProperty);

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

    private static Window? _mainWindow;
    private static readonly Dictionary<Window, HollowHost> Instances = new();

    private int _maxToasts;

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
        ToastsCollection ??= new AvaloniaList<Toast.Toast>();
        _maxToasts = GetToastLimit(window);
        var toastLoc = GetToastLocation(window);
        
        e.NameScope.Get<ItemsControl>("PART_ToastPresenter").HorizontalAlignment =
            toastLoc == ToastLocation.BottomLeft
                ? HorizontalAlignment.Left
                : HorizontalAlignment.Right;
      
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

    public static Task ShowToast(string title, string content, NotificationType? type = NotificationType.Info, TimeSpan? duration = null, Action? onClicked = null) =>
        ShowToast(new ToastModel(
            title,
            content,
            type ?? NotificationType.Info,
            duration ?? TimeSpan.FromSeconds(4),
            onClicked));

    public static Task ShowToast(Window window, string title, string content, NotificationType? type = NotificationType.Info, TimeSpan? duration = null,
        Action? onClicked = null) =>
        ShowToast(window, new ToastModel(
            title,
            content,
            type ?? NotificationType.Info,
            duration ?? TimeSpan.FromSeconds(4),
            onClicked));

    public static async Task ClearToast(Toast.Toast toast)
    {
        var wasRemoved = await Task.Run(async () =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                toast.Animate(OpacityProperty, 1d, 0d, TimeSpan.FromMilliseconds(300));
                toast.Animate(MarginProperty, new Thickness(), new Thickness(0, 50, 0, -50),
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

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (VisualRoot is not Window w) return;
        Instances.Remove(w);
        _mainWindow = Instances.FirstOrDefault().Key;
    }
}