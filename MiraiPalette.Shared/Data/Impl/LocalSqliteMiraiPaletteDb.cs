using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiraiPalette.Shared.Data.Impl;

public partial class LocalSqliteMiraiPaletteDb : MiraiPaletteDb
{
    public static string DbName { get; } = "mirai_palette.db";

    public static string DbPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

    public LocalSqliteMiraiPaletteDb(DbContextOptions<LocalSqliteMiraiPaletteDb> options) : base(options)
    {

    }
}

public class LocalSqliteMiraiPaletteDbFactory
    : IDesignTimeDbContextFactory<LocalSqliteMiraiPaletteDb>
{
    public LocalSqliteMiraiPaletteDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalSqliteMiraiPaletteDb>();

        // 设计时专用的 SQLite(临时文件)
        optionsBuilder.UseSqlite("Data Source=design_time.db");

        return new LocalSqliteMiraiPaletteDb(optionsBuilder.Options);
    }
}
