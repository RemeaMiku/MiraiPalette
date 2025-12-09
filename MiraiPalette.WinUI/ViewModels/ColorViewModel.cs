using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using MiraiPalette.WinUI.Essentials;
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

    public string Hex => Color.ToHex(false);

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;

    public ColorViewModel()
    {

    }

    [Obsolete("Use the parameterless constructor instead.")]
    public ColorViewModel(ColorEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name ?? string.Empty;
        Color = entity.Hex.ToColor();
    }
}
