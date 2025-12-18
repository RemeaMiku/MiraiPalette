using System.Linq;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Mappers;

public static class MiraiPaletteMapper
{
    extension(PaletteEntity entity)
    {
        public PaletteViewModel ToViewModel()
        {
            return new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description ?? string.Empty,
                FolderId = entity.FolderId ?? FolderViewModel.AllPalettes.Id,
                Colors = new(entity.Colors.OrderByDescending(c => c.Id).Select(c => c.ToViewModel())),
                TagIds = new(entity.Tags.Select(t => t.Id)),
            };
        }

        /// <summary>
        /// Maps a PaletteViewModel back to a PaletteEntity. (Colors and Tags are not mapped here)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public PaletteEntity FromViewModel(PaletteViewModel viewModel)
        {
            entity.Id = viewModel.Id;
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.FolderId = FolderViewModel.IsVirtualFolder(viewModel.FolderId) ? null : viewModel.FolderId;
            return entity;
        }
    }
}
