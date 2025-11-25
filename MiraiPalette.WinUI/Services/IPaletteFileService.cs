using System.Collections.Generic;
using System.Threading.Tasks;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services;

public interface IPaletteFileService
{
    public string[] SupportedImportFileExtensions { get; }

    public (string Description, IList<string> Extensions) SupportedExportFileTypes { get; }

    public Task<PaletteViewModel?> Import(string path);

    public Task Export(PaletteViewModel palette, string path);
}
