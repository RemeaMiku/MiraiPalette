using System.Text.Json.Serialization;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.Data.Entities;
using MiraiPalette.Maui.Utilities;

namespace MiraiPalette.Maui.Models;

public partial class MiraiColorModel : ObservableObject
{
    public MiraiColorModel()
    {

    }

    public MiraiColorModel(MiraiColor miraiColor)
    {
        Id = miraiColor.Id;
        Name = miraiColor.Name;
        Color = Color.FromArgb(miraiColor.Hex);
    }

    public int Id { get; set; } = 0;

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public string Hex => Color.ToHex();

    [JsonIgnore]
    public string Rgb => Color.ToRgbString();

    [JsonIgnore]
    public string Hsl => Color.ToHslString();

    [ObservableProperty]
    [JsonConverter(typeof(ColorJsonConverter))]
    public partial Color Color { get; set; } = Color.FromArgb(Constants.DefaultColorAsHex);

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;
}
