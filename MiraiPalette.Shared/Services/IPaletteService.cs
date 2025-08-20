namespace MiraiPalette.Shared.Services;

public interface IPaletteService
{
    Task<IEnumerable<Palette>> GetAllPalettesAsync();

    Task<Palette?> GetPaletteByIdAsync(int id);

    Task AddPaletteAsync(Palette palette);

    Task UpdatePaletteAsync(Palette palette);

    Task DeletePaletteAsync(int id);

    Task DeleteAllPalettesAsync();
}
