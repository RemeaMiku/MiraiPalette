using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using MiraiPalette.WinUI.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteDetailPageViewModel : PageViewModel
{
    public PaletteDetailPageViewModel(IPaletteDataService paletteDataService)
    {
        _paletteDataService = paletteDataService;
    }

    partial void OnPaletteChanged(PaletteViewModel value)
    {
        value.Colors.CollectionChanged += PaletteColors_CollectionChanged;
    }

    private void PaletteColors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateColorSelectionCommand.Execute(null);
    }

    [RelayCommand]
    async Task UpdatePalette()
    {
        if(IsBusy)
        {
            await Current.ShowConfirmDialogAsync("请稍候", "当前有正在进行的操作，请稍后再试。", false);
            return;
        }
        IsBusy = true;
        try
        {
            await _paletteDataService.UpdatePaletteAsync(Palette);
        }
        catch(Exception e)
        {
            await Current.ShowConfirmDialogAsync("更新调色板失败", "更新调色板时发生错误：" + e.Message);
        }
        IsBusy = false;
    }

    private readonly IPaletteDataService _paletteDataService;

    [ObservableProperty]
    public partial PaletteViewModel Palette { get; private set; } = null!;

    [ObservableProperty]
    public partial bool IsMultiSelectionEnabled { get; set; } = false;

    [ObservableProperty]
    public partial bool IsEditingColor { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedColors))]
    public partial ColorViewModel? SelectedColor { get; set; }

    Color _colorBefore = Colors.Transparent;

    public bool HasSelectedColors => SelectedColor is not null;

    [RelayCommand]
    public async Task Load(PaletteViewModel palette)
    {
        Palette = palette;
        palette.PropertyChanged += Palette_PropertyChanged;
        foreach(var color in palette.Colors)
            color.PropertyChanged += Color_PropertyChanged;
    }

    private void Color_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ColorViewModel.Name))
            UpdatePaletteCommand.Execute(null);
    }

    private async void Palette_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is nameof(PaletteViewModel.Title) && string.IsNullOrWhiteSpace(Palette.Title))
        {
            var title = (await _paletteDataService.GetPaletteAsync(Palette.Id))!.Title;
            Palette.Title = title;
            return;
        }
        if(e.PropertyName is nameof(PaletteViewModel.Title) or nameof(PaletteViewModel.Description))
            UpdatePaletteCommand.Execute(null);
    }

    [RelayCommand]
    public void UpdateColorSelection(IList<object> selection)
    {
        selection ??= Array.Empty<object>();
        foreach(var color in Palette.Colors)
            color.IsSelected = selection.Contains(color);
        if(selection.Count == 1 && !IsMultiSelectionEnabled)
        {
            IsEditingColor = true;
            EditColor();
        }
        else
        {
            IsEditingColor = false;
        }
    }

    [RelayCommand]
    static void CopyColorToClipboard(ColorViewModel color)
    {
        var package = new DataPackage();
        package.SetText(color.Hex);
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    async Task AddColor()
    {
        Palette.Colors.Insert(0, new ColorViewModel()
        {
            Name = $"新颜色 {Palette.Colors.Count + 1}",
            Color = Colors.White
        });
        await UpdatePalette();
        SelectedColor = Palette.Colors[0];
        EditColor();
    }

    void EditColor()
    {
        if(SelectedColor is null)
            return;
        _colorBefore = SelectedColor.Color;
    }

    [RelayCommand]
    void ResetEditingColor()
    {
        if(SelectedColor is null)
            return;
        SelectedColor.Color = _colorBefore;
    }

    [RelayCommand]
    async Task SaveEditingColor()
    {
        if(SelectedColor is null)
            return;
        if(SelectedColor.Color != _colorBefore)
            await UpdatePalette();
        SelectedColor = null;
        _colorBefore = Colors.Transparent;
    }

    partial void OnSelectedColorChanging(ColorViewModel? oldValue, ColorViewModel? newValue)
    {
        if(IsEditingColor && oldValue is not null && newValue is not null)
        {
            oldValue.Color = _colorBefore;
        }
    }

    [RelayCommand]
    async Task ExitColorEditing()
    {
        if(SelectedColor is null)
            return;
        if(SelectedColor.Color != _colorBefore)
        {
            var isConfirmed = await Current.ShowConfirmDialogAsync("保存更改", $"颜色 \"{SelectedColor.Name}\" 已更改，是否保存更改？");
            if(isConfirmed)
                await UpdatePalette();
            else
                ResetEditingColor();
        }
        SelectedColor = null;
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedColors(ColorViewModel? currentColor)
    {
        if(currentColor is null || currentColor.IsSelected)
        {
            var selectedColors = Palette.Colors.Where(c => c.IsSelected).ToArray();
            var message = selectedColors.Length == 1 ?
                $"确定要删除颜色 \"{selectedColors[0].Name}\"({selectedColors[0].Hex}) 吗？" :
                $"确定要删除所选的 {selectedColors.Length} 个颜色吗？";
            var isConfirmed = await Current.ShowConfirmDialogAsync("删除颜色", message);
            if(!isConfirmed)
                return;
            foreach(var color in selectedColors)
                Palette.Colors.Remove(color);
            await UpdatePalette();
        }
        else
        {
            var isConfirmed = await Current.ShowConfirmDialogAsync("删除颜色", $"确定要删除颜色 \"{currentColor.Name}\"({currentColor.Hex}) 吗？");
            if(!isConfirmed)
                return;
            Palette.Colors.Remove(currentColor);
            await UpdatePalette();
        }
    }

    [RelayCommand]
    async Task DeletePalette()
    {
        var isConfirmed = await Current.ShowConfirmDialogAsync("删除调色板", $"确定要删除调色板 \"{Palette.Title}\" 吗？");
        if(!isConfirmed)
            return;
        await UpdatePalette();
        // 导航回上一级
        Current.NavigateTo(NavigationTarget.Back);
    }

    partial void OnIsEditingColorChanged(bool value)
    {
        Trace.WriteLine($"IsEditingColor changed to {value}");
    }
}