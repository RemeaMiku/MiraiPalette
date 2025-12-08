using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiraiPalette.Shared.Essentials;
using MiraiPalette.WinUI.Mappers;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services.Impl;

public class DesignTimeStorageService : IMiraiPaletteStorageService
{
    private readonly Dictionary<int, PaletteEntity> _paletteEntities;
    private readonly Dictionary<int, FolderEntity> _folderEntities;
    private readonly Dictionary<int, TagEntity> _tagEntities;

    private const int _folderCount = 16;
    private const int _tagCount = 16;
    private const int _paletteCount = 128;
    private const int _paletteMaxColorCount = 16;

    private static IEnumerable<int> RandomSequence(int count, int maxValue)
    {
        if(count > maxValue + 1)
            throw new ArgumentException("count不能大于可用数字总数（maxValue + 1）。");

        // 1. 创建候选列表 [1, maxValue]
        var source = Enumerable.Range(1, maxValue).ToArray();

        // 2. Fisher–Yates 洗牌
        var random = new Random();
        for(int i = source.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1); // [0, i]
            (source[i], source[j]) = (source[j], source[i]);
        }

        // 3. 取前 count 个
        return source.Take(count);
    }


    public DesignTimeStorageService()
    {
        _folderEntities = [];
        _paletteEntities = [];
        _tagEntities = [];
        for(int id = 1; id <= _folderCount; id++)
        {
            _folderEntities[id] = new()
            {
                Id = id,
                Name = $"Folder Name {id}",
            };
        }

        for(int id = 1; id <= _tagCount; id++)
        {
            _tagEntities[id] = new()
            {
                Id = id,
                Name = $"Tag Name {id}"
            };
        }

        var totalColorCount = 0;
        for(int paletteId = 1; paletteId <= _paletteCount; paletteId++)
        {
            int? folderId = Random.Shared.Next(0, _folderCount + 1) == 0 ? null : Random.Shared.Next(1, _folderCount + 1);

            var tagCount = Random.Shared.Next(_tagCount);
            var tagIds = RandomSequence(tagCount, _tagCount);

            var palette = new PaletteEntity
            {
                Id = paletteId,
                Name = $"Palette Name {paletteId}",
                Description = $"Palette Description {paletteId}",
                FolderId = folderId,
                Tags = [.. tagIds.Select(i => _tagEntities[i])]
            };

            if(folderId.HasValue)
            {
                palette.Folder = _folderEntities[folderId.Value];
                _folderEntities[folderId.Value].Palettes.Add(palette);
            }

            var colorCount = Random.Shared.Next(_paletteMaxColorCount);
            var colorHexs = PaletteGenerator.GenerateRandomHexColor(colorCount).ToArray();
            for(int i = 0; i < colorCount; i++)
            {
                var colorId = totalColorCount + i + 1;
                palette.Colors.Add(new()
                {
                    Id = colorId,
                    Name = $"Color Name {colorId}",
                    Hex = colorHexs[i],
                    Palette = palette,
                    PaletteId = paletteId
                });
            }
            _paletteEntities[paletteId] = palette;


            foreach(var tagId in tagIds)
                _tagEntities[tagId].Palettes.Add(palette);

            totalColorCount += colorCount;
        }
    }

    public Task<int> AddPaletteAsync(PaletteViewModel palette)
    {
        var entity = new PaletteEntity().FromViewModel(palette);
        if(entity.FolderId.HasValue)
        {
            var folder = _folderEntities[entity.FolderId.Value];
            folder.Palettes.Add(entity);
            entity.Folder = folder;
        }
        var newId = _paletteEntities.Keys.Max() + 1;
        entity.Id = newId;
        _paletteEntities[newId] = entity;
        return Task.FromResult(newId);
    }

    public Task DeleteFolderAsync(int id) => throw new System.NotImplementedException();
    public Task DeletePaletteAsync(int paletteId) => throw new System.NotImplementedException();
    public Task DeletePalettesAsync(IEnumerable<int> paletteIds) => throw new System.NotImplementedException();
    public Task DeleteTagAsync(int id) => throw new System.NotImplementedException();
    public Task<IEnumerable<FolderViewModel>> GetAllFoldersAsync()
    {
        var models = _folderEntities.Values.Select(MiraiFolderMapper.ToViewModel);
        return Task.FromResult(models);
    }

    public Task<IEnumerable<PaletteViewModel>> GetAllPalettesAsync()
    {
        var models = _paletteEntities.Values.Select(MiraiPaletteMapper.ToViewModel);
        return Task.FromResult(models);
    }

    public Task<IEnumerable<TagViewModel>> GetAllTagsAsync() => throw new System.NotImplementedException();
    public Task<FolderViewModel?> GetFolderAsync(int id) => throw new System.NotImplementedException();
    public Task<PaletteViewModel?> GetPaletteAsync(int paletteId) => throw new System.NotImplementedException();
    public Task<TagViewModel?> GetTagAsync(int id) => throw new System.NotImplementedException();
    public Task UpdateFolderAsync(FolderViewModel folder) => throw new System.NotImplementedException();
    public Task UpdatePaletteAsync(PaletteViewModel palette) => throw new System.NotImplementedException();
    public Task UpdateTagAsync(TagViewModel tag) => throw new System.NotImplementedException();
    public Task<IEnumerable<PaletteViewModel>> GetPalettesByFolderAsync(int folderId)
    {
        if(folderId < 0)
        {
            if(folderId == FolderViewModel.AllPalettes.Id)
            {
                var all = _paletteEntities.Values.Select(MiraiPaletteMapper.ToViewModel);
                return Task.FromResult(all);
            }
        }
        var models = _folderEntities[folderId].Palettes.Select(MiraiPaletteMapper.ToViewModel);
        return Task.FromResult(models);
    }

    public Task<int> AddFolderAsync(FolderViewModel folder)
    {
        var entity = new FolderEntity().FromViewModel(folder);
        var newId = _folderEntities.Keys.Max() + 1;
        entity.Id = newId;
        folder.Id = newId;
        _folderEntities[newId] = entity;
        return Task.FromResult(newId);
    }
}
