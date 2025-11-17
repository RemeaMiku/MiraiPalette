using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MiraiPalette.Shared.Formats;
using MiraiPalette.Shared.Formats.Aco;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services.Impl;

public class PaletteFileService : IPaletteFileService
{
    const string _acoExtension = ".aco";
    const string _acoTypeDescription = "Adobe Color Swatch (*.aco)";

    public string[] SupportedImportFileExtensions => [_acoExtension];

    public (string, IList<string>) SupportedExportFileTypes => (_acoTypeDescription, [_acoExtension]);

    public async Task Export(PaletteViewModel palette, string path)
    {
        var extenion = Path.GetExtension(path).ToLower();
        switch(extenion)
        {
            case _acoExtension:
                var acoFile = new AcoFile()
                {
                    Colors = [.. palette.Colors.Select(c => AcoColor.FromHex(c.Hex, c.Name))],
                };
                await Task.Run(() => acoFile.Save(path));
                break;
            default:
                throw new NotSupportedException($"The file extension '{extenion}' is not supported for export.");
        }
    }

    public async Task<PaletteViewModel?> Import(string path)
    {
        var extenion = Path.GetExtension(path).ToLower();
        switch(extenion)
        {
            case _acoExtension:
                var acoFile = await Task.Run(() => AcoFile.Load(path));
                var palette = PaletteFormatConverter.FromAcoFile(acoFile);
                return new PaletteViewModel(palette)
                {
                    Title = Path.GetFileNameWithoutExtension(path),
                };
            default:
                return null;
        }
    }
}
