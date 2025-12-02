using CommunityToolkit.WinUI.Helpers;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Mappers;

public static class MiraiColorMapper
{
    extension(ColorEntity entity)
    {
        public ColorViewModel ToViewModel()
        {
            return new ColorViewModel
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Color = entity.Hex.ToColor()
            };
        }

        public ColorEntity FromViewModel(ColorViewModel viewModel)
        {
            entity.Id = viewModel.Id;
            entity.Name = viewModel.Name;
            entity.Hex = viewModel.Hex;
            return entity;
        }
    }
}