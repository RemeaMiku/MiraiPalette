using MiraiPalette.Shared.Entities.Abstract;

namespace MiraiPalette.Shared.Entities;

public class MiraiFolder : IHasTimestamps
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<MiraiPalette> Palettes { get; set; } = [];
}
