using System.Collections.Generic;
using System.Threading.Tasks;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services;

public interface IPaletteDataService
{
    public Task<IEnumerable<PaletteViewModel>> GetAllPalettesAsync();

    public Task<PaletteViewModel?> GetPaletteAsync(int paletteId);

    public Task<int> AddPaletteAsync(PaletteViewModel palette);

    public Task UpdatePaletteAsync(PaletteViewModel palette);

    public Task DeletePaletteAsync(int paletteId);

    public Task DeletePalettesAsync(IEnumerable<int> paletteIds);
}
