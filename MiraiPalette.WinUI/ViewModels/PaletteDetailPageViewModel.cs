using System.Collections.Generic;
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

    [ObservableProperty]
    public partial Color PreviewColor { get; set; } = Colors.Transparent;

    public bool HasSelectedColors => SelectedColor is not null;

    partial void OnIsEditingColorChanged(bool oldValue, bool newValue)
    {
        Trace.WriteLine(newValue);
    }

    [RelayCommand]
    public async Task Load(PaletteViewModel palette)
    {
        Palette = palette;
        palette.PropertyChanged += Palette_PropertyChanged;
        foreach(var color in palette.Colors)
            color.PropertyChanged += Color_PropertyChanged;
    }

    private async void Color_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await _paletteDataService.UpdatePaletteAsync(Palette);
    }

    private async void Palette_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await _paletteDataService.UpdatePaletteAsync(Palette);
    }

    [RelayCommand]
    public void UpdateColorSelection(IList<object> selection)
    {
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
        IsBusy = true;
        Palette.Colors.Insert(0, new ColorViewModel()
        {
            Name = $"新颜色 {Palette.Colors.Count + 1}",
            Color = Colors.White
        });
        await _paletteDataService.UpdatePaletteAsync(Palette);
        IsBusy = false;
        SelectedColor = Palette.Colors[0];
        EditColor();
    }

    void EditColor()
    {
        if(SelectedColor is null)
            return;
        PreviewColor = SelectedColor!.Color;
    }

    [RelayCommand]
    void ResetPreviewColor()
    {
        if(SelectedColor is null)
            return;
        PreviewColor = SelectedColor.Color;
    }

    [RelayCommand]
    async Task SaveEditingColor()
    {
        if(SelectedColor is null)
            return;
        SelectedColor.Color = PreviewColor;
        IsBusy = true;
        await _paletteDataService.UpdatePaletteAsync(Palette);
        IsBusy = false;
        SelectedColor = null;
    }

    [RelayCommand]
    void ExitColorEditing()
    {
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
            IsBusy = true;
            foreach(var color in selectedColors)
                Palette.Colors.Remove(color);
            await _paletteDataService.UpdatePaletteAsync(Palette);
            IsBusy = false;
        }
        else
        {
            var isConfirmed = await Current.ShowConfirmDialogAsync("删除颜色", $"确定要删除颜色 \"{currentColor.Name}\"({currentColor.Hex}) 吗？");
            if(!isConfirmed)
                return;
            IsBusy = true;
            Palette.Colors.Remove(currentColor);
            await _paletteDataService.UpdatePaletteAsync(Palette);
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DeletePalette()
    {
        var isConfirmed = await Current.ShowConfirmDialogAsync("删除调色板", $"确定要删除调色板 \"{Palette.Title}\" 吗？");
        if(!isConfirmed)
            return;
        IsBusy = true;
        await _paletteDataService.DeletePaletteAsync(Palette.Id);
        IsBusy = false;
        // 导航回上一级
        Current.NavigateTo(NavigationTarget.Back);
    }
}