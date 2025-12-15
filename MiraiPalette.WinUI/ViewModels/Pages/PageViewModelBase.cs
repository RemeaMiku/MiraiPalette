using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MiraiPalette.WinUI.Essentials.Navigation;

namespace MiraiPalette.WinUI.ViewModels;

public abstract partial class PageViewModelBase(IMessenger messenger) : ObservableObject
{
    protected IMessenger Messenger { get; } = messenger;

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; } = false;

    public bool IsNotBusy => !IsBusy;

    public override string ToString() => Title;

    protected virtual void OnNavigatedTo(object? parameter) { }

    protected virtual void OnNavigatedFrom() { }

    protected void Navigate(NavigationTarget target, object? parameter = null)
    {
        Messenger.Send(new NavigationMessage(target, parameter));
    }
}