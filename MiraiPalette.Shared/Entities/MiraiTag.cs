namespace MiraiPalette.Shared.Entities;

public class MiraiTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ColorHex { get; set; } = null!;

    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }

    public List<MiraiPalette> Palettes { get; set; } = [];
}
