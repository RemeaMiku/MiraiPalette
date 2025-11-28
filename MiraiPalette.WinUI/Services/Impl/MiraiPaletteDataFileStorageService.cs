using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services.Impl;

public class MiraiPaletteDataFileStorageService : IMiraiPaletteStorageService
{
    private const string _fileName = "palettes.mpd";

    private static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Mirai Palette");

    private const string _headerPrefix = "MIRAI_PALETTE_DATA_v";

    private const int _latestVersion = 2;

    private readonly Dictionary<int, PaletteEntity> _palettes = [];

    public MiraiPaletteDataFileStorageService()
    {
        ReadFile();
    }

    void SaveFile()
    {
        var builder = new StringBuilder();
        builder.AppendLine(_headerPrefix + _latestVersion);
        builder.AppendLine();
        foreach((var id, var palette) in _palettes)
        {
            builder.AppendLine($"ID:{id}");
            builder.AppendLine(palette.Name);
            builder.AppendLine(palette.Description);
            builder.AppendLine("COLORS:");
            foreach(var color in palette.Colors)
            {
                builder.AppendLine($"{color.Name}\t{color.Hex}");
            }
            builder.AppendLine();
        }
        var content = builder.ToString();
        if(!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
        }
        var fullPath = Path.Combine(_filePath, _fileName);
        File.WriteAllText(fullPath, content);
    }

    void ReadTags()
    {

    }

    void ReadFile()
    {
        if(!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
        }
        var fullPath = Path.Combine(_filePath, _fileName);
        if(!File.Exists(fullPath))
        {
            SaveFile();
            return;
        }
        var lines = File.ReadAllLines(fullPath);
        if(lines.Length == 0)
            return;
        var header = lines[0];
        if(!header.StartsWith(_headerPrefix))
            return;
        var version = (int)double.Parse(new string([.. header.Skip(header.Length - _headerPrefix.Length)]));
        for(int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if(string.IsNullOrWhiteSpace(line))
                continue;
            if(line.StartsWith("ID:", StringComparison.CurrentCultureIgnoreCase))
            {
                var idStr = line[3..].Trim();
                if(!int.TryParse(idStr, out int id))
                    continue;
                var title = lines[++i].Trim();
                var description = lines[++i].Trim();
                var colorsLine = lines[++i].Trim();
                if(!colorsLine.Equals("COLORS:", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                var colors = new List<ColorEntity>();
                while(++i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    var colorLine = lines[i].Trim();
                    var parts = colorLine.Split('\t');
                    if(parts.Length == 2)
                    {
                        var colorName = parts[0].Trim();
                        var colorHex = parts[1].Trim();
                        if(colorHex.IsValidHexColor())
                            colors.Add(new() { Name = colorName, Hex = colorHex });
                    }
                    else if(parts.Length == 1)
                    {
                        var colorHex = parts[0].Trim();
                        if(colorHex.IsValidHexColor())
                            colors.Add(new() { Hex = colorHex });
                    }
                }
                var palette = new PaletteEntity
                {
                    Id = id,
                    Name = title,
                    Description = description,
                    Colors = colors
                };
                _palettes[id] = palette;
            }
        }
    }

    public Task<int> AddPaletteAsync(PaletteViewModel palette)
    {
        var nextId = 1;
        if(_palettes.Count > 0)
            nextId = _palettes.Keys.Max() + 1;
        palette.Id = nextId;
        var entityColors = palette.Colors.Select(c => new ColorEntity { Name = c.Name, Hex = c.Hex }).ToList();
        var entity = new PaletteEntity
        {
            Id = palette.Id,
            Name = palette.Title,
            Description = palette.Description,
            Colors = entityColors
        };
        _palettes.Add(nextId, entity);
        SaveFile();
        return Task.FromResult(nextId);
    }

    public Task DeletePaletteAsync(int paletteId)
    {
        if(_palettes.Remove(paletteId))
        {
            SaveFile();
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<PaletteViewModel>> GetAllPalettesAsync()
    {
        return Task.FromResult(_palettes.Values.Reverse().Select(p => new PaletteViewModel(p)));
    }

    public Task<PaletteViewModel?> GetPaletteAsync(int paletteId)
    {
        return _palettes.TryGetValue(paletteId, out var palette)
            ? Task.FromResult<PaletteViewModel?>(new PaletteViewModel(palette))
            : Task.FromResult<PaletteViewModel?>(null);
    }

    public Task UpdatePaletteAsync(PaletteViewModel palette)
    {
        if(_palettes.ContainsKey(palette.Id))
        {
            var entityColors = palette.Colors.Select(c => new ColorEntity { Name = c.Name, Hex = c.Hex }).ToList();
            var entity = new PaletteEntity
            {
                Id = palette.Id,
                Name = palette.Title,
                Description = palette.Description,
                Colors = entityColors
            };
            _palettes[palette.Id] = entity;
            SaveFile();
        }
        return Task.CompletedTask;
    }

    public Task DeletePalettesAsync(IEnumerable<int> paletteIds)
    {
        foreach(var paletteId in paletteIds)
        {
            _palettes.Remove(paletteId);
        }
        SaveFile();
        return Task.CompletedTask;
    }
}
