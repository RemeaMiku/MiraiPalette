namespace MiraiPalette.Shared.Entities;

public class MiraiColor
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string Hex { get; set; } = null!;

    public int Order { get; set; }

    public int PaletteId { get; set; }
    public MiraiPalette Palette { get; set; } = null!;
}