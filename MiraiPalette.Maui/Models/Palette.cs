using System.Text.Json.Serialization;

namespace MiraiPalette.Maui.Models;

public class Palette
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<MiraiColor> Colors { get; set; } = [];

    [JsonIgnore]
    public bool IsSelected { get; set; }
}
