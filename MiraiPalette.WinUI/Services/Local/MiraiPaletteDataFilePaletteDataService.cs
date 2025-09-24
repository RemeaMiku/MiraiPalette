using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using MiraiPalette.Shared.Essentials;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services.Local;

public class MiraiPaletteDataFilePaletteDataService : IPaletteDataService
{
    private const string _fileName = "palettes.mpd";

    private static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Mirai Palette");

    private const string _headerFormat = "MIRAI_PALETTE_DATA_v{0}";

    private const string _version = "1.0";

    private readonly Dictionary<int, PaletteViewModel> _palettes = [];

    public MiraiPaletteDataFilePaletteDataService()
    {
        var random = new Random();
        for(int i = 0; i < 96; i++)
        {
            var count = random.Next(0, 16);
            var colors = PaletteGenerator.GenerateRandomHexColor(count).Select((hex, index)
                => new ColorViewModel { Id = index + 1, Name = $"Color {index + 1}", Color = hex.ToColor() });
            var palette = new PaletteViewModel()
            {
                Id = i + 1,
                Title = $"Palette {i + 1}",
                Description = $"This is palette {i + 1} with {count} colors.",
                Colors = new ObservableCollection<ColorViewModel>(colors)
            };
            _palettes.Add(palette.Id, palette);
        }
        SaveFile();
        ReadFile();
    }

    void SaveFile()
    {
        var builder = new StringBuilder();
        builder.AppendLine(string.Format(_headerFormat, _version));
        builder.AppendLine();
        foreach((var id, var palette) in _palettes)
        {
            builder.AppendLine($"ID:{id}");
            builder.AppendLine(palette.Title);
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
        if(header != string.Format(_headerFormat, _version))
            return;
        for(int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if(string.IsNullOrWhiteSpace(line))
                continue;
            if(line.StartsWith("ID:", System.StringComparison.CurrentCultureIgnoreCase))
            {
                var idStr = line.Substring(3).Trim();
                if(!int.TryParse(idStr, out int id))
                    continue;
                var title = lines[++i].Trim();
                var description = lines[++i].Trim();
                var colorsLine = lines[++i].Trim();
                if(!colorsLine.Equals("COLORS:", System.StringComparison.CurrentCultureIgnoreCase))
                    continue;
                var colors = new ObservableCollection<ColorViewModel>();
                while(++i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    var colorLine = lines[i].Trim();
                    var parts = colorLine.Split('\t');
                    if(parts.Length == 2)
                    {
                        var colorName = parts[0].Trim();
                        var colorHex = parts[1].Trim();
                        colors.Add(new ColorViewModel { Name = colorName, Color = colorHex.ToColor() });
                    }
                }
                var palette = new PaletteViewModel
                {
                    Id = id,
                    Title = title,
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
        _palettes.Add(nextId, palette);
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
        return Task.FromResult<IEnumerable<PaletteViewModel>>(_palettes.Values);
    }

    public Task<PaletteViewModel?> GetPaletteAsync(int paletteId)
    {
        _palettes.TryGetValue(paletteId, out var palette);
        return Task.FromResult(palette);
    }

    public Task UpdatePaletteAsync(PaletteViewModel palette)
    {
        if(_palettes.ContainsKey(palette.Id))
        {
            _palettes[palette.Id] = palette;
            SaveFile();
        }
        return Task.CompletedTask;
    }
}
