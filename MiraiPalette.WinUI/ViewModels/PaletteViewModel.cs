using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    public ObservableCollection<ColorViewModel> Colors { get; init; } = [];

    [ObservableProperty]
    public partial int FolderId { get; set; }

    public ObservableCollection<int> TagIds { get; init; } = [];

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public PaletteViewModel()
    {

    }

    [Obsolete("Use the parameterless constructor instead.")]
    public PaletteViewModel(PaletteEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Description = entity.Description ?? string.Empty;
        Colors = new(entity.Colors.Select(c => new ColorViewModel(c)));
        TagIds = new(entity.Tags.Select(t => t.Id));
    }

    public override string ToString()
        => Name;
}