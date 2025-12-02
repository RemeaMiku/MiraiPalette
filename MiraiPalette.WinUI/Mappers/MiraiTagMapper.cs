using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Mappers;

public static class MiraiTagMapper
{
    extension(TagEntity entity)
    {
        public TagViewModel ToViewModel()
        {
            return new TagViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public TagEntity FromViewModel(TagViewModel model)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            return entity;
        }
    }
}
