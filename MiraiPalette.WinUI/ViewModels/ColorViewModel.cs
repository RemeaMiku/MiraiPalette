using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ColorViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Hex))]
    public partial Color Color { get; set; }

    public string Hex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
}
