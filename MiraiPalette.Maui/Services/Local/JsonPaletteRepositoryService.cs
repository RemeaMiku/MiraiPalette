using System.Text.Json;
using Microsoft.Extensions.Logging;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Utilities;

namespace MiraiPalette.Maui.Services.Local;

public class JsonPaletteRepositoryService(ILogger<JsonPaletteRepositoryService> logger) : IPaletteRepositoryService
{
    private bool _isInitialized = false;

    private readonly ILogger _logger = logger;

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "palettes.json");

    private List<Palette> _palettes = [];

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() }
    };

    private readonly ColorJsonConverter _colorJsonConverter = new();

    private async Task SaveToJsonAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_palettes, _jsonSerializerOptions);
            await File.WriteAllTextAsync(_filePath, json);
            _logger.LogInformation("Saved {Count} palettes to palettes.json file at {FilePath}", _palettes.Count, _filePath);
        }
        catch(Exception e)
        {
            _logger.LogError("Failed to save palettes to palettes.json file at {FilePath}:{Message}", _filePath, e.Message);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        await InitAsync();
        var palette = _palettes.FirstOrDefault(p => p.Id == id);
        if(palette is null)
        {
            _logger.LogWarning("Palette with ID {Id} not found", id);
            return;
        }
        _palettes.Remove(palette);
        await SaveToJsonAsync();
    }

    public async Task<Palette?> GetAsync(int id)
    {
        await InitAsync();
        var palette = _palettes.FirstOrDefault(p => p.Id == id);
        if(palette is null)
        {
            _logger.LogWarning("Palette with ID {Id} not found", id);
        }
        return palette;
    }

    public async Task InitAsync()
    {
        if(_isInitialized)
            return;
        if(!File.Exists(_filePath))
        {
            await File.WriteAllTextAsync(_filePath, "[]");
            _logger.LogInformation("Created new palettes.json file at {FilePath}", _filePath);
        }
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var palettes = JsonSerializer.Deserialize<List<Palette>>(json, _jsonSerializerOptions);
            if(palettes is null)
            {
                _logger.LogWarning("Failed to deserialize palettes.json file at {FilePath}", _filePath);
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
            throw;
        }
        _isInitialized = true;
    }

    public async Task<List<Palette>> ListAsync()
    {
        await InitAsync();
        return _palettes;
    }

    public async Task<int> SaveAsync(Palette palette)
    {
        await InitAsync();
        if(palette.Id == 0)
        {
            palette.Id = GetNextId();
            _palettes.Add(palette);
            _logger.LogInformation("Added new palette with ID {Id}", palette.Id);
        }
        else
        {
            var index = _palettes.FindIndex(p => p.Id == palette.Id);
            if(index == -1)
            {
                _logger.LogError("Palette with ID {Id} not found", palette.Id);
                throw new ArgumentException("Palette not found", nameof(palette));
            }
            _palettes[index] = palette;
            _logger.LogInformation("Updated palette with ID {Id}", palette.Id);
        }
        await SaveToJsonAsync();
        return palette.Id;
    }

    private int GetNextId()
    {
        return _palettes.Count == 0 ? 1 : _palettes.Max(p => p.Id) + 1;
    }
}
