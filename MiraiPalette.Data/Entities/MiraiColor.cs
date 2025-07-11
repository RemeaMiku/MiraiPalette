namespace MiraiPalette.Data.Entities;

public class MiraiColor
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Hex { get; set; } = string.Empty;

    public int PaletteId { get; set; }
}