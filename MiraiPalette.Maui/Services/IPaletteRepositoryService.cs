using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.Services;

public interface IPaletteRepositoryService
{
    public Task<List<MiraiPaletteModel>> ListPalettesAsync();

    public Task<MiraiPaletteModel?> SelectPaletteAsync(int paletteId);

    public Task<int> InsertPaletteAsync(MiraiPaletteModel paletteModel);

    public Task UpdatePaletteAsync(MiraiPaletteModel paletteModel);

    public Task<int> InsertColorAsync(int paletteId, MiraiColorModel colorModel);

    public Task UpdateColorAsync(MiraiColorModel colorModel);

    public Task DeleteColorAsync(int colorId);

    public Task DeletePaletteAsync(int paletteId);
}
