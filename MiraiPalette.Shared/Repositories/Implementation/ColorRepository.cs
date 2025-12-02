
using Microsoft.EntityFrameworkCore;
using MiraiPalette.Shared.Data;
using MiraiPalette.Shared.Entities;

namespace MiraiPalette.Shared.Repositories.Implementation;

public class ColorRepository : IColorRepository
{
    public ColorRepository(MiraiPaletteDb db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _db.Database.Migrate();
    }

    private readonly MiraiPaletteDb _db;

    public async Task<int> AddColorAsync(MiraiColor color)
    {
        ArgumentNullException.ThrowIfNull(color);
        if(color.Id != 0)
            throw new ArgumentException("Color ID must be zero for new colors.", nameof(color));
        _db.Add(color);
        await _db.SaveChangesAsync();
        return color.Id;
    }

    public async Task DeleteColorAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
        var color = _db.Colors.Find(id) ?? throw new KeyNotFoundException($"Color with ID {id} not found.");
        _db.Remove(color);
        await _db.SaveChangesAsync();
    }

    public async Task<MiraiColor> GetColorByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
        return await _db.Colors.Include(c => c.Palette).FirstAsync(c => c.Id == id) ??
            throw new KeyNotFoundException($"Color with ID {id} not found.");
    }

    public async Task UpdateColorAsync(MiraiColor color)
    {
        ArgumentNullException.ThrowIfNull(color);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(color.Id, 0, nameof(color.Id));
        _db.Update(color);
        await _db.SaveChangesAsync();
    }
}
