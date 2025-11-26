namespace MiraiPalette.Shared.Entities;

public class MiraiFolder
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Palette> Palettes { get; set; } = [];

    public DateTimeOffset CreateAt { get; set; }

    public DateTimeOffset UpdateAt { get; set; }
}
