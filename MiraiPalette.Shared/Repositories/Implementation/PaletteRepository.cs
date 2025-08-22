using Microsoft.EntityFrameworkCore;
using MiraiPalette.Shared.Data;

namespace MiraiPalette.Shared.Repositories.Implementation;

public class PaletteRepository(MiraiPaletteDb db) : IPaletteRepository
{
    private readonly MiraiPaletteDb _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task<int> AddPaletteAsync(Palette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentOutOfRangeException.ThrowIfNotEqual(palette.Id, 0, nameof(palette));
        await _db.AddAsync(palette);
        await _db.SaveChangesAsync();
        return palette.Id;
    }

    public Task DeleteAllPalettesAsync()
    {
        _db.RemoveRange(_db.Palettes);
        return _db.SaveChangesAsync();
    }


    public async Task DeletePaletteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
        var palette = await _db.Palettes.FindAsync(id) ?? throw new KeyNotFoundException($"Palette with ID {id} not found.");
        _db.Remove(palette);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Palette>> GetAllPalettesAsync()
    {
        return await Task.Run(() => _db.Palettes.Include(p => p.Colors).ToList());
    }

    public async Task<Palette> GetPaletteByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0, nameof(id));
        return await _db.Palettes.Include(p => p.Colors).FirstAsync(p => p.Id == id) ?? throw new KeyNotFoundException($"Palette with ID {id} not found.");
    }

    public Task UpdatePaletteAsync(Palette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(palette.Id, 0, nameof(palette.Id));
        if(!_db.Palettes.Any(p => p.Id == palette.Id))
            throw new KeyNotFoundException($"Palette with ID {palette.Id} not found.");
        _db.Update(palette);
        return _db.SaveChangesAsync();
    }
}