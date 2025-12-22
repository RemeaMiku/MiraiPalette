using MiraiPalette.Shared.Entities.Abstract;

namespace MiraiPalette.Shared.Entities;

public class MiraiTag : IHasTimestamps
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ColorHex { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<MiraiPalette> Palettes { get; set; } = [];
}
