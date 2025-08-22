using MiraiPalette.Maui.Models;
using MiraiPalette.Shared.Entities;
using MiraiPalette.Shared.Repositories;

namespace MiraiPalette.Maui.Services.Local;

public class DbPaletteService(IPaletteRepository paletteRepository, IColorRepository colorRepository) : IPaletteService
{
    private readonly IPaletteRepository _paletteRepository = paletteRepository ?? throw new ArgumentNullException(nameof(paletteRepository));
    private readonly IColorRepository _colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));

    public async Task DeleteColorAsync(int colorId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(colorId, 0, nameof(colorId));
        await _colorRepository.DeleteColorAsync(colorId);
    }

    public async Task DeletePaletteAsync(int paletteId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paletteId, 0, nameof(paletteId));
        await _paletteRepository.DeletePaletteAsync(paletteId);
    }

    public async Task<int> InsertColorAsync(int paletteId, MiraiColorModel colorModel)
    {
        ArgumentNullException.ThrowIfNull(colorModel, nameof(colorModel));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paletteId, 0, nameof(paletteId));
        if(colorModel.Id != 0)
            throw new ArgumentException("Color ID must be zero for new colors.", nameof(colorModel));
        var palette = await _paletteRepository.GetPaletteByIdAsync(paletteId);
        var color = SetEntityFromModel(new(), colorModel);
        color.PaletteId = palette.Id;
        var newColorId = await _colorRepository.AddColorAsync(color);
        colorModel.Id = newColorId;
        return newColorId;
    }

    public async Task<int> InsertPaletteAsync(MiraiPaletteModel paletteModel)
    {
        ArgumentNullException.ThrowIfNull(paletteModel, nameof(paletteModel));
        if(paletteModel.Id != 0)
            throw new ArgumentException("Palette ID must be zero for new palettes.", nameof(paletteModel));
        var palette = SetEntityFromModel(new(), paletteModel);
        var newPaletteId = await _paletteRepository.AddPaletteAsync(palette);
        paletteModel.Id = newPaletteId;
        foreach(var colorModel in paletteModel.Colors)
            await InsertColorAsync(newPaletteId, colorModel);
        return newPaletteId;
    }

    public async Task<List<MiraiPaletteModel>> ListPalettesAsync()
    {
        var palettes = await _paletteRepository.GetAllPalettesAsync();
        var paletteModels = palettes.Select(t => new MiraiPaletteModel(t));
        return [.. paletteModels];
    }

    public async Task<MiraiPaletteModel?> SelectPaletteAsync(int paletteId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paletteId, 0, nameof(paletteId));
        var palette = await _paletteRepository.GetPaletteByIdAsync(paletteId);
        return new(palette);
    }

    public async Task UpdateColorAsync(MiraiColorModel colorModel)
    {
        ArgumentNullException.ThrowIfNull(colorModel, nameof(colorModel));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(colorModel.Id, 0, nameof(colorModel.Id));
        var color = await _colorRepository.GetColorByIdAsync(colorModel.Id) ?? throw new KeyNotFoundException($"Color with ID {colorModel.Id} not found.");
        color = SetEntityFromModel(color, colorModel);
        await _colorRepository.UpdateColorAsync(color);
    }

    public async Task UpdatePaletteAsync(MiraiPaletteModel paletteModel)
    {
        ArgumentNullException.ThrowIfNull(paletteModel, nameof(paletteModel));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paletteModel.Id, 0, nameof(paletteModel.Id));
        var palette = await _paletteRepository.GetPaletteByIdAsync(paletteModel.Id) ?? throw new KeyNotFoundException($"Palette with ID {paletteModel.Id} not found.");
        palette = SetEntityFromModel(palette, paletteModel);
        await _paletteRepository.UpdatePaletteAsync(palette);
    }

    private static MiraiColor SetEntityFromModel(MiraiColor entity, MiraiColorModel model)
    {
        entity.Id = model.Id;
        entity.Name = model.Name;
        entity.Hex = model.Hex;
        return entity;
    }

    private static Palette SetEntityFromModel(Palette entity, MiraiPaletteModel model)
    {
        entity.Id = model.Id;
        entity.Name = model.Name;
        entity.Description = model.Description;
        return entity;
    }
}