using CommunityToolkit.Maui.Core.Extensions;
using MiraiPalette.Maui.Models;
using MiraiPalette.Shared.Entities;

namespace MiraiPalette.Maui.Services.Local;

public class TestPaletteService : IPaletteService
{
    private readonly List<Palette> _palettes =
    [
        new Palette
        {
            Id = 1,
            Name = "Hatsune Miku",
            Description="Hatsune Miku's color palette",
        },
        new Palette
        {
            Id = 2,
            Name = "Kagamine Rin",
            Description="Kagamine Rin's color palette",
        },
        new Palette
        {
            Id = 3,
            Name = "Kagamine Len",
            Description="Kagamine Len's color palette",
        },
        new Palette
        {
            Id = 4,
            Name = "Megurine Luka",
            Description="Megurine Luka's color palette",
        },
    ];

    private readonly List<MiraiColor> _colors =
    [
        new MiraiColor
        {
            Id = 1,
            Name = "Main",
            Hex = "#39c5bb",
            PaletteId=1,
        },
        new MiraiColor
        {
            Id = 2,
            Name = "Accent",
            Hex = "#f9a8d4",
            PaletteId=1,
        },
        new MiraiColor
        {
            Id = 3,
            Name = "Dark",
            Hex = "#1e1e1e",
            PaletteId=1,
        },
        new MiraiColor
        {
            Id = 4,
            Name = "Light",
            Hex = "#f5f5f5",
            PaletteId=1,
        },
        new MiraiColor
        {
            Id = 5,
            Name = "Main",
            Hex = "#f9a8d4",
            PaletteId=2,
        },
        new MiraiColor
        {
            Id = 6,
            Name = "Accent",
            Hex = "#39c5bb",
            PaletteId=2,
        },
        new MiraiColor
        {
            Id = 7,
            Name = "Dark",
            Hex = "#1e1e1e",
            PaletteId=2,
        },
        new MiraiColor
        {
            Id = 8,
            Name = "Light",
            Hex = "#f5f5f5",
            PaletteId=2,
        },
        new MiraiColor
        {
            Id = 9,
            Name = "Main",
            Hex = "#f9a8d4",
            PaletteId=3,
        },
        new MiraiColor
        {
            Id = 10,
            Name = "Accent",
            Hex = "#39c5bb",
            PaletteId=3,
        },
        new MiraiColor
        {
            Id = 11,
            Name = "Dark",
            Hex = "#1e1e1e",
            PaletteId=3,
        },
        new MiraiColor
        {
            Id = 12,
            Name = "Light",
            Hex = "#f5f5f5",
            PaletteId=3,
        },
        new MiraiColor
        {
            Id = 13,
            Name = "Main",
            Hex = "#f9a8d4",
            PaletteId=4,
        },
        new MiraiColor
        {
            Id = 14,
            Name = "Accent",
            Hex = "#39c5bb",
            PaletteId=4,
        },
        new MiraiColor
        {
            Id = 15,
            Name = "Dark",
            Hex = "#1e1e1e",
            PaletteId=4,
        },
        new MiraiColor
        {
            Id = 16,
            Name = "Light",
            Hex = "#f5f5f5",
            PaletteId=4,
        },
    ];

    public async Task<int> InsertColorAsync(int paletteId, MiraiColorModel colorModel)
    {
        return await Task.Run(() =>
        {
            var palette = _palettes.FirstOrDefault(p => p.Id == paletteId) ?? throw new ArgumentException("Palette not found", nameof(paletteId));
            var colorId = _colors.Count > 0 ? _colors.Max(c => c.Id) + 1 : 1;
            _colors.Add(new MiraiColor
            {
                Id = colorId,
                Name = colorModel.Name,
                Hex = colorModel.Color.ToHex(),
                PaletteId = paletteId,
            });
            return colorId;
        });
    }

    public async Task DeleteColorAsync(int colorId)
    {
        await Task.Run(() =>
        {
            var color = _colors.FirstOrDefault(c => c.Id == colorId) ?? throw new ArgumentException("Color not found", nameof(colorId));
            _colors.Remove(color);
        });
    }

    public async Task DeletePaletteAsync(int paletteId)
    {
        await Task.Run(() =>
        {
            _palettes.RemoveAt(_palettes.FindIndex(p => p.Id == paletteId));
            _colors.RemoveAll(c => c.PaletteId == paletteId);
        });
    }

    public async Task<int> InsertPaletteAsync(MiraiPaletteModel paletteModel)
    {
        return await Task.Run(() =>
        {
            var paletteId = _palettes.Count > 0 ? _palettes.Max(p => p.Id) + 1 : 1;
            _palettes.Add(new Palette
            {
                Id = paletteId,
                Name = paletteModel.Name,
                Description = paletteModel.Description,
            });
            foreach(var color in paletteModel.Colors)
            {
                _colors.Add(new MiraiColor
                {
                    Id = color.Id,
                    Name = color.Name,
                    Hex = color.Color.ToHex(),
                    PaletteId = paletteId,
                });
            }
            return paletteId;
        });
    }

    public async Task<List<MiraiPaletteModel>> ListPalettesAsync()
    {
        return await Task.Run(() => _palettes.Select(p => new MiraiPaletteModel(p)
        {
            Colors = _colors.Where(c => c.PaletteId == p.Id).Select(c => new MiraiColorModel(c)).ToObservableCollection(),
        }).ToList());
    }

    public async Task<MiraiPaletteModel?> SelectPaletteAsync(int paletteId)
    {
        return await Task.Run(() =>
        {
            var palette = _palettes.FirstOrDefault(p => p.Id == paletteId);
            if(palette is not null)
            {
                var colors = _colors.Where(c => c.PaletteId == paletteId);
                return new MiraiPaletteModel(palette)
                {
                    Colors = colors.Select(c => new MiraiColorModel(c)).ToObservableCollection(),
                };
            }
            else
                return null;
        });
    }

    public async Task UpdateColorAsync(MiraiColorModel colorModel)
    {
        ArgumentNullException.ThrowIfNull(colorModel);
        await Task.Run(() =>
        {
            var color = _colors.FirstOrDefault(c => c.Id == colorModel.Id);
            if(color is not null)
            {
                color.Name = colorModel.Name;
                color.Hex = colorModel.Color.ToHex();
            }
        });
    }

    public async Task UpdatePaletteAsync(MiraiPaletteModel paletteModel)
    {
        ArgumentNullException.ThrowIfNull(paletteModel);
        await Task.Run(() =>
        {
            var palette = _palettes.FirstOrDefault(p => p.Id == paletteModel.Id);
            if(palette is not null)
            {
                palette.Name = paletteModel.Name;
                palette.Description = paletteModel.Description;
            }
        });
    }
}