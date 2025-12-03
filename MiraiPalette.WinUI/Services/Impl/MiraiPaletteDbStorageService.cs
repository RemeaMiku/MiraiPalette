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
            palette.Id = 0;
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

        return list.Select(MiraiPaletteMapper.ToViewModel);
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
        foreach(var c in model.Colors)
            entity.Colors.Add(new MiraiColor().FromViewModel(c));

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
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if(entity is null)
            return;

        entity.FromViewModel(model);

        //
        // 更新 Colors（最安全做法：整体替换）
        //
        entity.Colors.Clear();
        foreach(var c in model.Colors)
            entity.Colors.Add(new MiraiColor().FromViewModel(c));

        //
        // 更新 Tags（多对多：重新绑定）
        //
        entity.Tags.Clear();

        if(model.TagIds.Count > 0)
        {
            var newTags = await _db.Tags
                .Where(t => model.TagIds.Contains(t.Id))
                .ToListAsync();

            foreach(var t in newTags)
                entity.Tags.Add(t);
        }

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
        var list = await _db.Folders
            .OrderBy(f => f.CreatedAt)
            .ToListAsync();

        return list.Select(MiraiFolderMapper.ToViewModel);
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

        return list.Select(MiraiTagMapper.ToViewModel);
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
}