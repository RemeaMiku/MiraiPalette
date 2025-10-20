using System.Collections.Generic;
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
        AutoColors.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasColors));
        ManualColors.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasColors));
    }

    [ObservableProperty]
    public required partial string ImagePath { get; set; }

    [ObservableProperty]
    public partial int ColorCount { get; set; } = 4;

    public int MinimumColorCount { get; } = 4;

    public int MaximumColorCount { get; } = 16;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PointerPixel))]
    public partial Point PointerPositionOnImage { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasColors))]
    public partial ObservableCollection<ColorViewModel> AutoColors { get; set; } = [];

    public ObservableCollection<ColorViewModel> ManualColors { get; } = [];

    public bool HasColors => AutoColors.Count + ManualColors.Count > 0;

    public Color PointerPixel => ImagePixels is null || !IsPickingColor ? Colors.Transparent : ImagePixels.FromOriginalCoord((int)PointerPositionOnImage.X, (int)PointerPositionOnImage.Y);

    public required ImagePixels ImagePixels { get; set; }

    [ObservableProperty]
    public partial bool IsPickingColor { get; set; }

    readonly IPaletteDataService _paletteDataService;

    [RelayCommand]
    async Task ExtractPalette()
    {
        IsBusy = true;
        var result = await Task.Run(() => new ImagePaletteExtractor().Extract(ImagePixels.PixelData.Select(c => (c.R, c.G, c.B)), ColorCount));
        IsBusy = false;
        AutoColors = [.. result.Select((c) =>
            {
                var color = Color.FromArgb(255, c.R, c.G, c.B);
                return new ColorViewModel
                {
                    Color = color,
                    Name = $"{color.ToHex(false)} ({c.Percentage:0.0}%)",
                };
            }
        )];
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
        ManualColors.Add(color);
    }

    [RelayCommand]
    void DeleteCurrentOrSelectedColors(ColorViewModel color)
    {
        var sourceList = AutoColors.Contains(color) ? AutoColors : ManualColors;
        if(!color.IsSelected)
        {
            sourceList.Remove(color);
            return;
        }
        for(int i = sourceList.Count - 1; i >= 0; i--)
            if(sourceList[i].IsSelected)
                sourceList.RemoveAt(i);
    }

    [RelayCommand]
    void SelectAutoColors(IList<object> selectedColors)
    {
        foreach(var color in AutoColors)
            color.IsSelected = false;
        foreach(var color in selectedColors.Cast<ColorViewModel>())
            color.IsSelected = true;
    }

    [RelayCommand]
    void SelectManualColors(IList<object> selectedColors)
    {
        foreach(var color in ManualColors)
            color.IsSelected = false;
        foreach(ColorViewModel color in selectedColors.Cast<ColorViewModel>())
            color.IsSelected = true;
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