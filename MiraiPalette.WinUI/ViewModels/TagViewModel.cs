using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using MiraiPalette.Shared.Entities;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class TagViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Color Color { get; set; }

    public TagViewModel()
    {

    }

    public TagViewModel(MiraiTag tag)
    {
        Id = tag.Id;
        Name = tag.Name;
        Color = tag.ColorHex.ToColor();
    }
}