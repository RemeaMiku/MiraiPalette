using MiraiPalette.Shared.Entities;

namespace MiraiPalette.Shared.Repositories;

public interface IColorRepository
{
    Task<MiraiColor> GetColorByIdAsync(int id);

    Task<int> AddColorAsync(MiraiColor color);

    Task UpdateColorAsync(MiraiColor color);

    Task DeleteColorAsync(int id);
}
