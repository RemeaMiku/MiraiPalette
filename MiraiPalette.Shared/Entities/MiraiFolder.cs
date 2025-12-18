using MiraiPalette.Shared.Entities.Abstract;

namespace MiraiPalette.Shared.Entities;

public class MiraiFolder : IHasTimeStamp
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<MiraiPalette> Palettes { get; set; } = [];
}
