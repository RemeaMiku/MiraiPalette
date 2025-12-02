using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Mappers;

public static class MiraiFolderMapper
{
    extension(FolderEntity entity)
    {
        public FolderViewModel ToViewModel()
        {
            return new FolderViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public FolderEntity FromViewModel(FolderViewModel viewModel)
        {
            entity.Id = viewModel.Id;
            entity.Name = viewModel.Name;
            return entity;
        }
    }
}
