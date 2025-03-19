using System.Text.Json.Serialization;
using CommunityToolkit.Maui.Core.Extensions;
using MiraiPalette.Maui.Utilities;

namespace MiraiPalette.Maui.Models;

public class MiraiColor
{

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public string Hex => Color.ToHex();

    [JsonIgnore]
    public string Rgb => Color.ToRgbString();

    [JsonIgnore]
    public string Hsl => Color.ToHslString();

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Color { get; set; } = Color.FromArgb(Constans.DefaultColorAsHex);
}
