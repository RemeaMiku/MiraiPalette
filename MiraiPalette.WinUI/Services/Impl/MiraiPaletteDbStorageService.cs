using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiraiPalette.Shared.Data;
using MiraiPalette.Shared.Entities;
using MiraiPalette.WinUI.Mappers;
using MiraiPalette.WinUI.ViewModels;

namespace MiraiPalette.WinUI.Services.Impl;

public class MiraiPaletteDbStorageService : IMiraiPaletteStorageService
{

    public static string DbName { get; } = "mirai_palette.db";

    public static string DbFolderPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Mirai Palette");

    public MiraiPaletteDbStorageService(MiraiPaletteDb db)
    {
        _db = db;
        if(!Directory.Exists(DbFolderPath))
            Directory.CreateDirectory(DbFolderPath);
        _db.Database.Migrate();

        if(File.Exists(Path.Combine(DbFolderPath, MiraiPaletteDataFileStorageService.FileName)))
            MpdFileToDb();
    }

    //TODO 临时数据迁移
    private void MpdFileToDb()
    {
        var service = new MiraiPaletteDataFileStorageService();
        foreach(var palette in service.GetAllPalettesAsync().Result)
        {
            service.DeletePaletteAsync(palette.Id);
            _ = AddPaletteAsync(palette);
        }
        File.Delete(Path.Combine(DbFolderPath, MiraiPaletteDataFileStorageService.FileName));
    }

    private readonly MiraiPaletteDb _db;

    public async Task<IEnumerable<PaletteViewModel>> GetAllPalettesAsync()
    {
        var list = await _db.Palettes
            .Include(p => p.Colors)
            .Include(p => p.Tags)
            .ToListAsync();

        return list
            .OrderByDescending(p => p.CreatedAt)
            .Select(MiraiPaletteMapper.ToViewModel);
    }

    public async Task<PaletteViewModel?> GetPaletteAsync(int paletteId)
    {
        var entity = await _db.Palettes
            .Include(p => p.Colors)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == paletteId);

        return entity?.ToViewModel();
    }

    public async Task<int> AddPaletteAsync(PaletteViewModel model)
    {
        var entity = new PaletteEntity().FromViewModel(model);

        // Colors
        for(int i = 0; i < model.Colors.Count; i++)
        {
            entity.Colors.Add(new MiraiColor()
            {
                Order = i
            }.FromViewModel(model.Colors[i]));
        }

        // Tags: 多对多
        if(model.TagIds.Count > 0)
        {
            var tags = await _db.Tags
                .Where(t => model.TagIds.Contains(t.Id))
                .ToListAsync();

            entity.Tags.AddRange(tags);
        }

        _db.Palettes.Add(entity);
        await _db.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdatePaletteAsync(PaletteViewModel model)
    {
        var entity = await _db.Palettes
            .Include(p => p.Colors)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if(entity == null)
            return;

        // 1️⃣ 更新 Palette 自身属性
        entity.FromViewModel(model);

        // 2️⃣ 处理 Colors（重点）
        var existingColors = entity.Colors.ToDictionary(c => c.Id);
        var incomingColors = model.Colors;

        for(int i = 0; i < incomingColors.Count; i++)
        {
            var colorModel = incomingColors[i];

            if(colorModel.Id > 0 && existingColors.TryGetValue(colorModel.Id, out var colorEntity))
            {
                // 已有 Color → 更新属性 + Order
                colorEntity
                    .FromViewModel(colorModel)
                    .Order = i;
            }
            else
            {
                // 新 Color
                entity.Colors.Add(new MiraiColor()
                {
                    Order = i
                }.FromViewModel(colorModel));
            }
        }

        // 3️⃣ 删除被移除的 Color
        var incomingIds = incomingColors
            .Where(c => c.Id > 0)
            .Select(c => c.Id)
            .ToHashSet();

        var toRemove = entity.Colors
            .Where(c => c.Id > 0 && !incomingIds.Contains(c.Id))
            .ToList();

        _db.Colors.RemoveRange(toRemove);

        await _db.SaveChangesAsync();
    }


    public async Task DeletePaletteAsync(int paletteId)
    {
        var entity = await _db.Palettes.FindAsync(paletteId);
        if(entity is null)
            return;

        _db.Palettes.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeletePalettesAsync(IEnumerable<int> paletteIds)
    {
        var list = await _db.Palettes
            .Where(p => paletteIds.Contains(p.Id))
            .ToListAsync();

        if(list.Count == 0)
            return;

        _db.Palettes.RemoveRange(list);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<FolderViewModel>> GetAllFoldersAsync()
    {
        var folders = await _db.Folders
            .OrderBy(f => f.Order)
            .ToListAsync();

        var result = folders.Select(MiraiFolderMapper.ToViewModel);

        return result;
    }


    public async Task<FolderViewModel?> GetFolderAsync(int id)
    {
        var entity = await _db.Folders.FindAsync(id);
        return entity?.ToViewModel();
    }

    public async Task UpdateFolderAsync(FolderViewModel model)
    {
        var entity = await _db.Folders.FindAsync(model.Id);
        if(entity is null)
            return;

        entity.FromViewModel(model);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteFolderAsync(int id)
    {
        var entity = await _db.Folders.FindAsync(id);
        if(entity is null)
            return;

        _db.Folders.Remove(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<TagViewModel>> GetAllTagsAsync()
    {
        var list = await _db.Tags
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        return list
            .OrderByDescending(t => t.CreatedAt)
            .Select(MiraiTagMapper.ToViewModel);
    }

    public async Task<TagViewModel?> GetTagAsync(int id)
    {
        var entity = await _db.Tags.FindAsync(id);
        return entity?.ToViewModel();
    }

    public async Task UpdateTagAsync(TagViewModel model)
    {
        var entity = await _db.Tags.FindAsync(model.Id);
        if(entity == null)
            return;

        entity.FromViewModel(model);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(int id)
    {
        var t = await _db.Tags.FindAsync(id);
        if(t == null)
            return;

        // Tag 删除时不删除 Palette，也不影响 Palette
        // EF 中间表级联只删 PaletteTags 行，不删 Palette
        _db.Tags.Remove(t);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<PaletteViewModel>> GetPalettesByFolderAsync(int folderId)
    {
        var query = _db.Palettes
            .Include(p => p.Colors)
            .Include(p => p.Tags)
            .AsQueryable();

        if(FolderViewModel.IsVirtualFolder(folderId))
        {
            if(folderId == FolderViewModel.Unassigned.Id)
                query = query.Where(p => p.FolderId == null);
        }
        else
            query = query.Where(p => p.FolderId == folderId);

        var list = await query.ToListAsync();

        return list
            .OrderByDescending(p => p.CreatedAt)
            .Select(MiraiPaletteMapper.ToViewModel);
    }

    public async Task<int> AddFolderAsync(FolderViewModel folder)
    {
        var entity = new FolderEntity().FromViewModel(folder);
        _db.Folders.Add(entity);
        await _db.SaveChangesAsync();
        folder.Id = entity.Id;
        return entity.Id;
    }
}