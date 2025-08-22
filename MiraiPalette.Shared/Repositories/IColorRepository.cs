namespace MiraiPalette.Shared.Repositories;
public interface IColorRepository
{
    Task<Color> GetColorByIdAsync(int id);

    Task<int> AddColorAsync(Color color);

    Task UpdateColorAsync(Color color);

    Task DeleteColorAsync(int id);
}
