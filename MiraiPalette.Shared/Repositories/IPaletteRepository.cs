namespace MiraiPalette.Shared.Repositories;

public interface IPaletteRepository
{
    Task<IEnumerable<Entities.MiraiPalette>> GetAllPalettesAsync();

    Task<Entities.MiraiPalette> GetPaletteByIdAsync(int id);

    Task<int> AddPaletteAsync(Entities.MiraiPalette palette);

    Task UpdatePaletteAsync(Entities.MiraiPalette palette);

    Task DeletePaletteAsync(int id);

    Task DeleteAllPalettesAsync();
}
