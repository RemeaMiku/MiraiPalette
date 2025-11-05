using System.Threading.Tasks;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services;

public interface IPaletteFileService
{
    public Task<PaletteViewModel?> Import(string path);

    public Task Export(PaletteViewModel palette, string path);
}
