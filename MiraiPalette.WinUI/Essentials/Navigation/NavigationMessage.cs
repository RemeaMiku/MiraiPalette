using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MiraiPalette.WinUI.Essentials.Navigation;

public class NavigationMessage(NavigationTarget target, object? parameter = null) : RequestMessage<object?>
{
    public NavigationTarget Target { get; set; } = target;

    public object? Parameter { get; set; } = parameter;
}
