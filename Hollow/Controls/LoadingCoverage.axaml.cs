using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Hollow.Controls;

public class LoadingCoverage : UserControl
{
    public static readonly StyledProperty<string> BackgroundTextProperty =
        AvaloniaProperty.Register<LoadingCoverage, string>(nameof(BackgroundText));

    public string BackgroundText
    {
        get => GetValue(BackgroundTextProperty);
        set => SetValue(BackgroundTextProperty, value);
    }
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<LoadingCoverage, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<LoadingCoverage, string>(nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
}