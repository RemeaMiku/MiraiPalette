using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiraiPalette.Maui.Models;

public partial class MiraiPaletteModel : ObservableObject
{
    public MiraiPaletteModel()
    {

    }

    public MiraiPaletteModel(Palette palette)
    {
        Id = palette.Id;
        Name = palette.Name;
        Description = palette.Description;
    }

    public int Id { get; set; } = 0;

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<MiraiColorModel> Colors { get; set; } = [];

    [JsonIgnore]
    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;
}