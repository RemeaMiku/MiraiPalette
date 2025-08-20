using Microsoft.EntityFrameworkCore;

namespace MiraiPalette.Shared.Data;

public class MiraiPaletteDb : DbContext
{
    public DbSet<Palette> Palettes { get; set; } = null!;
    public DbSet<Color> Colors { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mirai_palette.db");
    }
}
