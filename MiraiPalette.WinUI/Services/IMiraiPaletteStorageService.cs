using System.Collections.Generic;
using System.Threading.Tasks;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services;

public interface IMiraiPaletteStorageService
{

    #region Basic MiraiPalette Crud

    public Task<IEnumerable<PaletteViewModel>> GetAllPalettesAsync();

    public Task<PaletteViewModel?> GetPaletteAsync(int paletteId);

    public Task<int> AddPaletteAsync(PaletteViewModel palette);

    public Task UpdatePaletteAsync(PaletteViewModel palette);

    public Task DeletePaletteAsync(int paletteId);

    public Task DeletePalettesAsync(IEnumerable<int> paletteIds);

    #endregion

    public Task<IEnumerable<FolderViewModel>> GetAllFoldersAsync();

    public Task<FolderViewModel?> GetFolderAsync(int id);

    public Task UpdateFolderAsync(FolderViewModel folder);

    public Task DeleteFolderAsync(int id);

    public Task<IEnumerable<TagViewModel>> GetAllTagsAsync();

    public Task<TagViewModel?> GetTagAsync(int id);

    public Task UpdateTagAsync(TagViewModel tag);

    public Task DeleteTagAsync(int id);
}
