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
                FolderId = entity.FolderId ?? 0,
                Colors = new(entity.Colors.Select(c => c.ToViewModel())),
                TagIds = new(entity.Tags.Select(t => t.Id)),
            };
        }

        public PaletteEntity FromViewModel(PaletteViewModel viewModel)
        {
            entity.Id = viewModel.Id;
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.FolderId = viewModel.FolderId == FolderViewModel.DefaultFolder.Id ? default : viewModel.FolderId;
            return entity;
        }
    }
}
