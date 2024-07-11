using Avalonia;
using Avalonia.Controls;

namespace Hollow.Controls;

public class ProhibitedCoverage : UserControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ProhibitedCoverage, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}