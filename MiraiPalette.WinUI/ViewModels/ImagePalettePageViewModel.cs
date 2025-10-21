using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using MiraiPalette.Shared.Essentials;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.Services;
using Windows.Foundation;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ImagePalettePageViewModel : PageViewModel
{
    public ImagePalettePageViewModel(IPaletteDataService paletteDataService)
    {
        _paletteDataService = paletteDataService;
        AutoColors.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(HasAutoColors));
            OnPropertyChanged(nameof(CanSavePalette));
        };
        ManualColors.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(HasManualColors));
            OnPropertyChanged(nameof(CanSavePalette));
        };
    }

    [ObservableProperty]
    public required partial string ImagePath { get; set; }

    [ObservableProperty]
    public partial int ColorCount { get; set; } = 4;

    public int MinimumColorCount { get; } = 4;

    public int MaximumColorCount { get; } = 16;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PointerPixel))]
    [NotifyPropertyChangedFor(nameof(PointerPixelHex))]
    public partial Point PointerPositionOnImage { get; set; }

    public ObservableCollection<ColorViewModel> AutoColors { get; } = [];

    public ObservableCollection<ColorViewModel> ManualColors { get; } = [];

    public bool HasAutoColors => AutoColors.Count > 0;

    public bool HasManualColors => ManualColors.Count > 0;

    public bool CanSavePalette => (HasAutoColors || HasManualColors) && IsNotBusy;

    public Color PointerPixel => ImagePixels is null || !IsPickingColor ? Colors.Transparent : ImagePixels.FromOriginalCoord((int)PointerPositionOnImage.X, (int)PointerPositionOnImage.Y);

    public string PointerPixelHex => PointerPixel.ToHex(false);

    public required ImagePixels ImagePixels { get; set; }

    [ObservableProperty]
    public partial bool IsPickingColor { get; set; }

    readonly IPaletteDataService _paletteDataService;

    [RelayCommand]
    async Task ExtractPalette()
    {
        IsBusy = true;
        AutoColors.Clear();
        var result = await Task.Run(() => new ImagePaletteExtractor().Extract(ImagePixels.PixelData.Select(c => (c.R, c.G, c.B)), ColorCount).ToArray());
        var colors = result.Select(c =>
        {
            var color = Color.FromArgb(255, c.R, c.G, c.B);
            return new ColorViewModel
            {
                Color = color,
                Name = $"{color.ToHex(false)} ({c.Percentage:0.0}%)",
            };
        });
        foreach(var color in colors)
            AutoColors.Add(color);
        IsBusy = false;
    }

    [RelayCommand]
    void AddPickedColor()
    {
        if(!IsPickingColor)
            return;
        var pickedColor = PointerPixel;
        var color = new ColorViewModel
        {
            Name = pickedColor.ToHex(false),
            Color = pickedColor,
        };
        ManualColors.Insert(0, color);
    }

    [RelayCommand]
    async Task ClearAutoColors()
    {
        var confirmed = await Current.ShowConfirmDialogAsync("清除颜色", "确定要清除所有自动提取的颜色吗？");
        if(!confirmed)
            return;
        AutoColors.Clear();
    }

    [RelayCommand]
    async Task ClearManualColors()
    {
        var confirmed = await Current.ShowConfirmDialogAsync("清除颜色", "确定要清除所有手动吸取的颜色吗？");
        if(!confirmed)
            return;
        ManualColors.Clear();
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedColors(ColorViewModel color)
    {
        var sourceList = AutoColors.Contains(color) ? AutoColors : ManualColors;
        if(!color.IsSelected)
        {
            var confirmed = await Current.ShowConfirmDialogAsync("删除颜色", $"确定要删除颜色 {color.Name} 吗？");
            if(!confirmed)
                return;
            sourceList.Remove(color);
            return;
        }
        var confirmedAll = await Current.ShowConfirmDialogAsync("删除选中颜色", $"确定要删除所选的 {sourceList.Count(c => c.IsSelected)} 个颜色吗？");
        if(!confirmedAll)
            return;
        for(int i = sourceList.Count - 1; i >= 0; i--)
            if(sourceList[i].IsSelected)
                sourceList.RemoveAt(i);
    }

    [RelayCommand]
    static void ToggleColorSelection(ColorViewModel color)
    {
        color.IsSelected = !color.IsSelected;
    }

    [RelayCommand]
    async Task SavePalette()
    {
        var palette = new PaletteViewModel
        {
            Title = $"图像调色板 - {Path.GetFileNameWithoutExtension(ImagePath)}",
            Description = $"从图像 \"{ImagePath}\" 提取的调色板",
            Colors = [.. AutoColors, .. ManualColors]
        };
        IsBusy = true;
        await _paletteDataService.AddPaletteAsync(palette);
        IsBusy = false;
        Current.NavigateTo(NavigationTarget.Back);
    }
}