using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class TagViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Color Color { get; set; }
}