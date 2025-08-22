using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.Services.Local;

public class JsonPaletteService : IPaletteService
{
    private readonly ILogger _logger;

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "palettes.json");

    private readonly FileStream _fileStream;

    public JsonPaletteService(ILogger<JsonPaletteService> logger)
    {
        _logger = logger;
        //if(!Directory.Exists("data"))
        //{
        //    Directory.CreateDirectory("data");
        //    _logger.LogInformation("Created new data directory");
        //}
        if(!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
            _logger.LogInformation("Created new palettes.json file at {FilePath}", _filePath);
        }
        _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        try
        {
            var palettes = JsonSerializer.Deserialize<List<MiraiPaletteModel>>(_fileStream, _jsonSerializerOptions);
            if(palettes is null)
            {
                _logger.LogError("Failed to deserialize palettes.json file at {FilePath}", _filePath);
                throw new Exception("Failed to deserialize palettes.json file");
            }
            else
            {
                _palettes = palettes;
                _logger.LogInformation("Read {Count} palettes from palettes.json file at {FilePath}", _palettes.Count, _filePath);
            }
        }
        catch(Exception e)
        {
            _logger.LogError("Failed to read palettes.json file at {FilePath}:{Message}", _filePath, e.Message);
            throw new Exception("Failed to read palettes.json file", e);
        }
    }

    private readonly List<MiraiPaletteModel> _palettes;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() }
    };

    public async Task<int> InsertPaletteAsync(MiraiPaletteModel paletteModel)
    {
        var paletteId = GetNextPaletteId();
        var palette = new MiraiPaletteModel
        {
            Id = paletteId,
            Name = paletteModel.Name,
            Description = paletteModel.Description,
            Colors = []
        };
        foreach(var colorModel in paletteModel.Colors)
        {
            var colorId = GetNextColorId();
            var color = new MiraiColorModel
            {
                Id = colorId,
                Name = colorModel.Name,
                Color = colorModel.Color
            };
            palette.Colors.Add(color);
        }
        _palettes.Add(palette);
        await SaveToJsonAsync();
        return paletteId;
    }

    public async Task<int> InsertColorAsync(int paletteId, MiraiColorModel colorModel)
    {
        var palette = GetPalette(paletteId);
        var colorId = GetNextColorId();
        var color = new MiraiColorModel
        {
            Id = colorId,
            Name = colorModel.Name,
            Color = colorModel.Color
        };
        palette.Colors.Add(color);
        await SaveToJsonAsync();
        return colorId;
    }

    private int GetNextPaletteId()
    {
        return _palettes.Count == 0 ? 1 : _palettes.Max(p => p.Id) + 1;
    }

    private int GetNextColorId()
    {
        if(_palettes.Count == 0)
            return 1;
        var allColors = _palettes.SelectMany(p => p.Colors);
        return !allColors.Any() ? 1 : allColors.Max(c => c.Id) + 1;
    }

    public async Task UpdateColorAsync(MiraiColorModel colorModel)
    {
        GetPaletteAndColorOfColorId(colorModel.Id, out var _, out var color);
        color.Name = colorModel.Name;
        color.Color = colorModel.Color;
        await SaveToJsonAsync();
    }

    private void GetPaletteAndColorOfColorId(int colorId, out MiraiPaletteModel paletteModel, out MiraiColorModel colorModel)
    {
        foreach(var palette in _palettes)
        {
            var color = palette.Colors.FirstOrDefault(c => c.Id == colorId);
            if(color is not null)
            {
                paletteModel = palette;
                colorModel = color;
                return;
            }
        }
        throw new ArgumentException("Color not found", nameof(colorId));
    }

    public async Task DeleteColorAsync(int colorId)
    {
        GetPaletteAndColorOfColorId(colorId, out var palette, out var color);
        palette.Colors.Remove(color);
        await SaveToJsonAsync();
    }

    public Task<List<MiraiPaletteModel>> ListPalettesAsync()
        => Task.FromResult(_palettes);

    public Task<MiraiPaletteModel?> SelectPaletteAsync(int paletteId)
        => Task.Run(() =>
        {
            var palette = _palettes.FirstOrDefault(p => p.Id == paletteId);
            if(palette is null)
            {
                _logger.LogWarning("Palette with ID {PaletteId} not found", paletteId);
                return null;
            }
            var copyPalette = new MiraiPaletteModel()
            {
                Id = palette.Id,
                Name = palette.Name,
                Description = palette.Description,
                Colors = palette.Colors
            };
            return copyPalette;
        });

    public async Task DeletePaletteAsync(int paletteId)
    {
        var palette = _palettes.FirstOrDefault(p => p.Id == paletteId) ?? throw new ArgumentException("Palette not found", nameof(paletteId));
        _palettes.Remove(palette);
        await SaveToJsonAsync();
    }

    public async Task UpdatePaletteAsync(MiraiPaletteModel paletteModel)
    {
        var palette = GetPalette(paletteModel.Id);
        palette.Name = paletteModel.Name;
        palette.Description = paletteModel.Description;
        await SaveToJsonAsync();
    }

    private MiraiPaletteModel GetPalette(int paletteId)
    {
        var palette = _palettes.FirstOrDefault(p => p.Id == paletteId);
        return palette is null ? throw new ArgumentException("Palette not found", nameof(paletteId)) : palette;
    }

    private async Task SaveToJsonAsync()
    {
        _fileStream.Seek(0, SeekOrigin.Begin);
        _fileStream.SetLength(0);
        if(_palettes.Count == 0)
        {
            _fileStream.Write(Encoding.UTF8.GetBytes("[]"));
        }
        else
        {
            await JsonSerializer.SerializeAsync(_fileStream, _palettes, _jsonSerializerOptions);
        }
        _fileStream.Flush();
    }
}