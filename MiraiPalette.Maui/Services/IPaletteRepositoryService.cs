using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.Services;

public interface IPaletteRepositoryService
{
    public Task InitAsync();

    public Task<List<Palette>> ListAsync();

    public Task<Palette?> GetAsync(int id);

    public Task<int> SaveAsync(Palette palette);

    public Task DeleteAsync(int id);
}
