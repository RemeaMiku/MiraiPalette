using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiraiPalette.Shared.Data.Impl;

public partial class LocalMiraiPaletteDb(DbContextOptions<LocalMiraiPaletteDb> options) : MiraiPaletteDb(options)
{
}

public class LocalMiraiPaletteDbFactory
    : IDesignTimeDbContextFactory<LocalMiraiPaletteDb>
{
    public LocalMiraiPaletteDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalMiraiPaletteDb>();

        // 设计时专用的 SQLite(临时文件)
        optionsBuilder.UseSqlite("Data Source=design_time.db");

        return new LocalMiraiPaletteDb(optionsBuilder.Options);
    }
}