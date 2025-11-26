namespace MiraiPalette.Shared.Entities;

public class MiraiTag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ColorHex { get; set; } = string.Empty;

    public List<Palette> Palettes { get; set; } = [];

    public DateTimeOffset CreateAt { get; set; }

    public DateTimeOffset UpdateAt { get; set; }
}
