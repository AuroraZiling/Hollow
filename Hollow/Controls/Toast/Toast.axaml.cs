using System;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using FluentIcons.Common;
using Hollow.Enums;
using Hollow.Resources;

namespace Hollow.Controls.Toast;

public class Toast : ContentControl
{
    protected override Type StyleKeyOverride => typeof(Toast);
    
    internal HollowHost Host { get; private set; } = null!;

    private readonly Timer _timer = new();

    private Action? _onClickedCallback;
    private Action? _onActionCallback;

    public Toast()
    {
        _timer.Elapsed += TimerOnElapsed!;
    }

    private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        await HollowHost.ClearToast(this);
    }
    
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<Toast, object?>(nameof(Icon));

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<Toast, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly StyledProperty<bool> ShowActionButtonProperty =
        AvaloniaProperty.Register<Toast, bool>(nameof(ShowActionButton));

    public bool ShowActionButton
    {
        get => GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }
    
    public static readonly StyledProperty<string> ActionButtonContentProperty =
        AvaloniaProperty.Register<Toast, string>(nameof(ActionButtonContent));

    public string ActionButtonContent
    {
        get => GetValue(ActionButtonContentProperty);
        set => SetValue(ActionButtonContentProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        e.NameScope.Get<Border>("PART_ToastCard").PointerPressed += ToastCardClickedHandler!;
        e.NameScope.Get<Button>("ButtonAction").Click += ButtonActionClicked!;
    }

    private void ButtonActionClicked(object sender, RoutedEventArgs e)
    {
        _onActionCallback?.Invoke();
    }

    private async void ToastCardClickedHandler(object o, PointerPressedEventArgs pointerPressedEventArgs)
    {
        _onClickedCallback?.Invoke();
        _onClickedCallback = null;
        await HollowHost.ClearToast(this);
    }
    
    // Icon Foreground Brushes
    // Note: it would be better to place them into a resource dictionary, but findResource performs slightly slower
    

    public void Initialize(ToastModel model, HollowHost host)
    {
        Host = host;
        Title = model.Title;
        Content = model.Content;
        if (model.ActionButtonContent != null || model.ActionButton != null)
        {
            ShowActionButton = true;
            ActionButtonContent = model.ActionButtonContent!;
            _onActionCallback = model.OnActionButtonClicked;
        }
        else
        {
            ShowActionButton = false;
            ActionButtonContent = "";
            _onActionCallback = null;
        }
        Icon = model.Type switch
        {
            NotificationType.Info => Symbol.Info,
            NotificationType.Success => Symbol.Check,
            NotificationType.Warning => Symbol.Warning,
            NotificationType.Error => Symbol.ErrorCircle,
            _ => Symbol.Info
        };
        Foreground = model.Type switch
        {
            NotificationType.Info => NotificationColor.InfoIconForeground,
            NotificationType.Success => NotificationColor.SuccessIconForeground,
            NotificationType.Warning => NotificationColor.WarningIconForeground,
            NotificationType.Error => NotificationColor.ErrorIconForeground,
            _ => NotificationColor.InfoIconForeground
        };
        _onClickedCallback = model.OnClicked;
       
        _timer.Interval = model.Lifetime?.TotalMilliseconds ?? TimeSpan.FromSeconds(6).TotalMilliseconds;
        _timer.Start();
        DockPanel.SetDock(this, Dock.Bottom);
    }
}