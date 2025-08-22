namespace MiraiPalette.Shared.Repositories;

public interface IPaletteRepository
{
    Task<IEnumerable<Palette>> GetAllPalettesAsync();

    Task<Palette> GetPaletteByIdAsync(int id);

    Task<int> AddPaletteAsync(Palette palette);

    Task UpdatePaletteAsync(Palette palette);

    Task DeletePaletteAsync(int id);

    Task DeleteAllPalettesAsync();
}
