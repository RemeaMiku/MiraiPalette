using Microsoft.EntityFrameworkCore;
using MiraiPalette.Shared.Entities;
using MiraiPalette.Shared.Entities.Abstract;

namespace MiraiPalette.Shared.Data;

public abstract class MiraiPaletteDb : DbContext
{
    public DbSet<Entities.MiraiPalette> Palettes { get; set; } = null!;
    public DbSet<MiraiColor> Colors { get; set; } = null!;
    public DbSet<MiraiTag> Tags { get; set; } = null!;
    public DbSet<MiraiFolder> Folders { get; set; } = null!;

    protected MiraiPaletteDb(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //
        // 1. Folder → Palettes   (cascade)
        //
        modelBuilder.Entity<Entities.MiraiPalette>()
            .HasOne(p => p.Folder)
            .WithMany(f => f.Palettes)
            .HasForeignKey(p => p.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        //
        // 2. Palette → Colors    (cascade)
        //
        modelBuilder.Entity<MiraiColor>()
            .HasOne(c => c.Palette)
            .WithMany(p => p.Colors)
            .HasForeignKey(c => c.PaletteId)
            .OnDelete(DeleteBehavior.Cascade);

        //
        // 3. Palette ↔ Tag 多对多（不级联删除 Tag）
        //
        modelBuilder.Entity<Entities.MiraiPalette>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Palettes)
            .UsingEntity<Dictionary<string, object>>(
                "PaletteTags",
                j => j
                    .HasOne<MiraiTag>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Cascade), // Palette 删除时删 join rows
                j => j
                    .HasOne<Entities.MiraiPalette>()
                    .WithMany()
                    .HasForeignKey("PaletteId")
                    .OnDelete(DeleteBehavior.Cascade)
            );

        // 字段长度/必填约束示例
        modelBuilder.Entity<Entities.MiraiPalette>()
            .Property(p => p.Name).IsRequired().HasMaxLength(200);

        modelBuilder.Entity<MiraiTag>()
            .Property(t => t.Name).IsRequired().HasMaxLength(100);

        modelBuilder.Entity<MiraiColor>()
            .Property(c => c.Hex).IsRequired().HasMaxLength(9);

        // 可选：为多对多关系设置中间表名和索引（EF Core 会自动生成表）
        modelBuilder.Entity<Entities.MiraiPalette>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Palettes)
            .UsingEntity(j => j.ToTable("PaletteTags"));
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTimeOffset.UtcNow;
        foreach(var entry in ChangeTracker.Entries())
        {
            if(entry.Entity is IHasTimeStamp)
            {
                if(entry.State == EntityState.Added)
                    entry.Property(nameof(IHasTimeStamp.CreatedAt)).CurrentValue = now;

                if(entry.State is EntityState.Added or EntityState.Modified)
                    entry.Property(nameof(IHasTimeStamp.UpdatedAt)).CurrentValue = now;
            }
        }
    }
}
