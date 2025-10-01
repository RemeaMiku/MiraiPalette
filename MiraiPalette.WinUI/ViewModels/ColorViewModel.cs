using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ColorViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Hex))]
    public partial Color Color { get; set; }

    public string Hex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;

    public ColorViewModel()
    {

    }

    public ColorViewModel(ColorEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Color = entity.Hex.ToColor();
    }
}
