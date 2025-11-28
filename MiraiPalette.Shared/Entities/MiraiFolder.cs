namespace MiraiPalette.Shared.Entities;

public class MiraiFolder
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }

    public List<MiraiPalette> Palettes { get; set; } = [];
}
