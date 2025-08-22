using Microsoft.EntityFrameworkCore;

namespace MiraiPalette.Shared.Data;

public abstract class MiraiPaletteDb : DbContext
{
    public DbSet<Palette> Palettes { get; set; } = null!;

    public DbSet<Color> Colors { get; set; } = null!;
}
