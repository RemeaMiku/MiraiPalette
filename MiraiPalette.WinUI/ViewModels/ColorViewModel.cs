using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ColorViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial Color Color { get; set; }

    public string Hex => Color.ToHex();
}
