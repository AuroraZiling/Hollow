using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Hollow.Views.Controls.Home;

public class StartGameNoticeButton : ContentControl
{
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<StartGameNoticeButton, string>(nameof(Title), string.Empty);

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
        AvaloniaProperty.Register<StartGameNoticeButton, ICommand>(nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}