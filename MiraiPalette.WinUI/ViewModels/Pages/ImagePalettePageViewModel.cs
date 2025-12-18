using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using MiraiPalette.Shared.Essentials;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.Essentials.ImagePalette;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;
using Windows.Foundation;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ImagePalettePageViewModel : PageViewModelBase
{
    public ImagePalettePageViewModel(IMiraiPaletteStorageService paletteDataService, IMessenger messenger) : base(messenger)
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
    public required partial FolderViewModel TargetFolder { get; set; }

    [ObservableProperty]
    public required partial Uri ImagePath { get; set; }

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

    public override void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        ArgumentNullException.ThrowIfNull(parameter);
        if(parameter is Dictionary<string, object> args
            && args.TryGetValue("ImagePath", out var imagePathObj)
            && imagePathObj is Uri uri
            && args.TryGetValue("Folder", out var folderObj)
            && folderObj is FolderViewModel folder)
        {
            ImagePath = uri;
            TargetFolder = folder;
        }
    }

    async partial void OnImagePathChanged(Uri value)
    {
        ImagePixels = await ImagePixelsExtractor.Default.ExtractImagePixelsAsync(value.LocalPath);
    }

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
    void Cancel()
    {
        Navigate(NavigationTarget.Back, TargetFolder);
    }

    [RelayCommand]
    async Task SavePalette()
    {
        var palette = new PaletteViewModel
        {
            Name = Path.GetFileNameWithoutExtension(ImagePath.LocalPath),
            Description = string.Format(ImagePalettePageStrings.ImagePaletteDescription, ImagePath),
            Colors = [.. AutoColors, .. ManualColors],
            FolderId = TargetFolder.Id
        };
        try
        {
            IsBusy = true;
            palette.Id = await _paletteDataService.AddPaletteAsync(palette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.CreatePalette_Title, ErrorMessages.CreatePalette_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
        Navigate(NavigationTarget.Main, TargetFolder);
    }
}