using CommunityToolkit.Mvvm.ComponentModel;

namespace MiraiPalette.WinUI.ViewModels;

public abstract partial class PageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; } = false;
}
