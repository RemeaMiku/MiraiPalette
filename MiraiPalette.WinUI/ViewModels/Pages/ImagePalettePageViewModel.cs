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
using MiraiPalette.WinUI.Essentials.ImagePalette;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;
using Windows.Foundation;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ImagePalettePageViewModel : PageViewModelBase
{
    public ImagePalettePageViewModel(IMiraiPaletteStorageService paletteDataService)
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
    public partial FolderViewModel TargetFolder { get; set; } = FolderViewModel.AllPalettes;

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

    readonly IMiraiPaletteStorageService _paletteDataService;

    [RelayCommand]
    async Task ExtractPalette()
    {
        IsBusy = true;
        AutoColors.Clear();
        var result = await Task.Run(() => new ImagePaletteExtractor().Extract(ImagePixels.PixelData.Select(c => (c.R, c.G, c.B)), ColorCount).ToArray());
        IsBusy = false;
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
        var confirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.AutoColorListClear_Title, DeleteConfirmStrings.AutoColorListClear_Message);
        if(!confirmed)
            return;
        AutoColors.Clear();
    }

    [RelayCommand]
    async Task ClearManualColors()
    {
        var confirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.ManualColorListClear_Title, DeleteConfirmStrings.ManualColorListClear_Message);
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
            color.IsSelected = selectedColors.Contains(color);
    }

    [RelayCommand]
    void SelectManualColors(IList<object> selectedColors)
    {
        foreach(var color in ManualColors)
            color.IsSelected = selectedColors.Contains(color);
    }

    [RelayCommand]
    async Task SavePalette()
    {
        var palette = new PaletteViewModel
        {
            Name = Path.GetFileNameWithoutExtension(ImagePath),
            Description = string.Format(ImagePalettePageStrings.ImagePaletteDescription, ImagePath),
            Colors = [.. AutoColors, .. ManualColors]
        };
        IsBusy = true;
        await _paletteDataService.AddPaletteAsync(palette);
        IsBusy = false;
        Current.NavigateTo(NavigationTarget.Back);
    }
}