using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Hollow.Controls;

public class StartGameButton : ContentControl
{
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<StartGameButton, string>(nameof(Title), string.Empty);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<StartGameButton, string>(nameof(Message), string.Empty);

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<StartGameButton, ICommand>(nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}