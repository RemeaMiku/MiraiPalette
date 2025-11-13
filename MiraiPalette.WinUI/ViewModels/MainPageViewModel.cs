using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Shared.Formats;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel : PageViewModel
{
    public MainPageViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IPaletteFileService paletteFileService)
    {
        _miraiPaletteStorageService = miraiPaletteStorageService;
        _paletteFileService = paletteFileService;
        SelectedPalettes.CollectionChanged += OnSelectedPalettesChanged;
        PaletteCommand = NavigateToPaletteCommand;
    }

    private void OnSelectedPalettesChanged(object? _, NotifyCollectionChangedEventArgs __)
    {
        OnPropertyChanged(nameof(HasSelectedPalettes));
        OnPropertyChanged(nameof(IsAllPalettesSelected));
    }

    [ObservableProperty]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    readonly IMiraiPaletteStorageService _miraiPaletteStorageService;

    readonly IPaletteFileService _paletteFileService;

    public ObservableCollection<PaletteViewModel> SelectedPalettes { get; } = [];

    [ObservableProperty]
    public partial PaletteViewModel CurrentPalette { get; set; } = null!;

    public bool HasSelectedPalettes => SelectedPalettes.Count > 0;

    public bool IsAllPalettesSelected => Palettes.Count > 0 && SelectedPalettes.Count == Palettes.Count;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [ObservableProperty]
    public partial ICommand PaletteCommand { get; set; }

    [RelayCommand]
    private void SelectAllPalettes()
    {
        if(!IsMultiSelectMode)
            throw new InvalidOperationException("Not in multi-select mode.");
        foreach(var palette in Palettes)
        {
            if(!palette.IsSelected)
            {
                palette.IsSelected = true;
                SelectedPalettes.Add(palette);
            }
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach(var palette in SelectedPalettes)
            palette.IsSelected = false;
        SelectedPalettes.Clear();
    }

    [RelayCommand]
    async Task Load()
    {
        Palettes.Clear();
        SelectedPalettes.Clear();
        IsBusy = true;
        Palettes = new(await _miraiPaletteStorageService.GetAllPalettesAsync());
        IsBusy = false;
    }

    partial void OnIsMultiSelectModeChanged(bool value)
    {
        if(value == true)
        {
            PaletteCommand = TogglePaletteSelectionCommand;
        }
        else
        {
            PaletteCommand = NavigateToPaletteCommand;
            ClearSelection();
        }
    }

    [RelayCommand]
    void SetCurrentPalette(PaletteViewModel palette)
    {
        CurrentPalette = palette;
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedPalettes(PaletteViewModel? palette)
    {
        if(palette is null || palette.IsSelected)
        {
            var message = SelectedPalettes.Count == 1 ?
            $"确定要删除调色板 \"{SelectedPalettes[0].Title}\" 吗？" :
            $"确定要删除所选的 {SelectedPalettes.Count} 个调色板吗？";
            var confirmed = await Current.ShowConfirmDialogAsync("删除调色板", message);
            if(!confirmed)
                return;
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePalettesAsync(SelectedPalettes.Select(p => p.Id));
            IsBusy = false;
        }
        else
        {
            var confirmed = await Current.ShowConfirmDialogAsync("删除调色板", $"确定要删除调色板 \"{palette.Title}\" 吗？");
            if(!confirmed)
                return;
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePaletteAsync(palette.Id);
            IsBusy = false;
        }
        await Load();
    }

    [RelayCommand]
    void TogglePaletteSelection(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            DeselectPalette(palette);
        else
            SelectPalette(palette);
    }

    [RelayCommand]
    void NavigateToPalette(PaletteViewModel palette)
    {
        Current.NavigateTo(NavigationTarget.Palette, palette);
        ClearSelection();
    }

    [RelayCommand]
    async Task AddPalette()
    {
        IsMultiSelectMode = false;
        var newPalette = new PaletteViewModel()
        {
            Title = Resources.DefaultPaletteTitle,
            Description = string.Empty,
            Colors = []
        };
        IsBusy = true;
        await _miraiPaletteStorageService.AddPaletteAsync(newPalette);
        Palettes.Insert(0, newPalette);
        IsBusy = false;
        NavigateToPalette(newPalette);
    }

    [RelayCommand]
    async Task AddPaletteFromFile()
    {
        IsMultiSelectMode = false;
        var path = await Current.PickFileToOpen("导入调色板文件", ".aco");
        if(path is null)
            return;
        if(!File.Exists(path))
        {
            await Current.ShowConfirmDialogAsync("导入调色板失败", $"文件路径\"{path}\"不存在。", false);
            return;
        }
        try
        {
            IsBusy = true;
            var palette = await _paletteFileService.Import(path);
            if(palette is null)
            {
                await Current.ShowConfirmDialogAsync("导入调色板失败", "未能导入调色板", false);
                return;
            }
            await _miraiPaletteStorageService.AddPaletteAsync(palette);
            Palettes.Insert(0, palette);
            NavigateToPalette(palette);
        }
        catch(Exception e)
        {
            await Current.ShowConfirmDialogAsync("导入调色板失败", "导入调色板时发生错误：" + e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void SelectPalette(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            throw new InvalidOperationException("Palette is already selected.");
        palette.IsSelected = true;
        SelectedPalettes.Add(palette);
    }

    [RelayCommand]
    void DeselectPalette(PaletteViewModel palette)
    {
        if(!palette.IsSelected)
            throw new InvalidOperationException("Palette is not selected.");
        palette.IsSelected = false;
        SelectedPalettes.Remove(palette);
    }

    [RelayCommand]
    async Task NavigateToImagePalette()
    {
        var path = await Current.PickFileToOpen("打开", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff");
        if(path is not null)
            Current.NavigateTo(NavigationTarget.ImagePalette, path);
        ClearSelection();
    }
}
