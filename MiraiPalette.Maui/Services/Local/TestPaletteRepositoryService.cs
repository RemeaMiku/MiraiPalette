using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.Services.Local;

public class TestPaletteRepositoryService : IPaletteRepositoryService
{
    private readonly Dictionary<int, Palette> _palettes = new()
    {
        {
            1,
            new Palette
            {
                Id = 1,
                Name = "Hatsune Miku",
                Description="Hatsune Miku's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Green",
                        Color = Color.FromArgb("#39c5bb"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Blue",
                        Color = Color.FromArgb("#01b0f0"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
        {
            2,
            new Palette
            {
                Id = 2,
                Name = "Kagamine Rin",
                Description="Kagamine Rin's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            3,
            new Palette
            {
                Id = 3,
                Name = "Kagamine Len",
                Description="Kagamine Len's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Len Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len White",
                        Color = Color.FromArgb("#ffffff"),
                        },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            4,
            new Palette
            {
                Id = 4,
                Name = "Megurine Luka",
                Description="Megurine Luka's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Megurine Luka Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Black",
                        Color = Color.FromArgb("#000000"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
         {
            5,
            new Palette
            {
                Id = 5,
                Name = "Hatsune Miku",
                Description="Hatsune Miku's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Green",
                        Color = Color.FromArgb("#39c5bb"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Blue",
                        Color = Color.FromArgb("#01b0f0"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
        {
            6,
            new Palette
            {
                Id = 6,
                Name = "Kagamine Rin",
                Description="Kagamine Rin's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            7,
            new Palette
            {
                Id = 7,
                Name = "Kagamine Len",
                Description="Kagamine Len's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Len Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len White",
                        Color = Color.FromArgb("#ffffff"),
                        },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            8,
            new Palette
            {
                Id = 8,
                Name = "Megurine Luka",
                Description="Megurine Luka's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Megurine Luka Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Black",
                        Color = Color.FromArgb("#000000"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
         {
            9,
            new Palette
            {
                Id = 9,
                Name = "Hatsune Miku",
                Description="Hatsune Miku's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Green",
                        Color = Color.FromArgb("#39c5bb"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Blue",
                        Color = Color.FromArgb("#01b0f0"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Hatsune Miku White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
        {
            10,
            new Palette
            {
                Id = 10,
                Name = "Kagamine Rin",
                Description="Kagamine Rin's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Rin Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            11,
            new Palette
            {
                Id = 11,
                Name = "Kagamine Len",
                Description="Kagamine Len's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Kagamine Len Yellow",
                        Color = Color.FromArgb("#f7e000"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Orange",
                        Color = Color.FromArgb("#f39800"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Kagamine Len White",
                        Color = Color.FromArgb("#ffffff"),
                        },
                    new MiraiColor
                    {
                        Name = "Kagamine Len Black",
                        Color = Color.FromArgb("#000000"),
                    },
                ]
            }
        },
        {
            12,
            new Palette
            {
                Id = 12,
                Name = "Megurine Luka",
                Description="Megurine Luka's color palette",
                Colors =
                [
                    new MiraiColor
                    {
                        Name = "Megurine Luka Pink",
                        Color = Color.FromArgb("#f7a9e6"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Red",
                        Color = Color.FromArgb("#e60033"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka Black",
                        Color = Color.FromArgb("#000000"),
                    },
                    new MiraiColor
                    {
                        Name = "Megurine Luka White",
                        Color = Color.FromArgb("#ffffff"),
                    },
                ]
            }
        },
    };

    public Task DeleteAsync(int id)
    {
        return Task.FromResult(_palettes.Remove(id));
    }

    public Task<Palette?> GetAsync(int id)
    {
        return Task.FromResult(_palettes.TryGetValue(id, out var palette) ? palette : null);
    }

    public Task InitAsync()
    {
        return Task.CompletedTask;
    }

    public Task<List<Palette>> ListAsync()
    {
        return Task.FromResult(_palettes.Values.ToList());
    }

    public Task<int> SaveAsync(Palette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        if(palette.Id == 0)
        {
            palette.Id = _palettes.Keys.Max() + 1;
            _palettes.Add(palette.Id, palette);
        }
        else
        {
            _palettes[palette.Id] = palette;
        }
        return Task.FromResult(palette.Id);
    }
}
