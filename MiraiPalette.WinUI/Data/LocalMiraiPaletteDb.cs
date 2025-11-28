using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using MiraiPalette.Shared.Data;

namespace MiraiPalette.WinUI.Data;

public class LocalMiraiPaletteDb : MiraiPaletteDb
{
    public static string DbName { get; } = "mirai_palette.db";

    public static string DbPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

    public LocalMiraiPaletteDb(DbContextOptions<LocalMiraiPaletteDb> options) : base(options)
    {

    }
}
