using Microsoft.EntityFrameworkCore;

namespace MiraiPalette.Shared.Data.Implementation;

public class LocalSqliteMiraiPaletteDb : MiraiPaletteDb
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mirai_palette.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Color>()
            .HasOne(c => c.Palette)
            .WithMany(p => p.Colors)
            .HasForeignKey(c => c.PaletteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
