using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.PageModels;

public partial class ImagePalettePageModel : ObservableObject
{
    public ImagePalettePageModel()
    {
        Colors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Colors));
    }

    public ObservableCollection<MiraiColorModel> Colors { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageSource))]
    public partial string ImagePath { get; set; } = string.Empty;

    public ImageSource? ImageSource => string.IsNullOrWhiteSpace(ImagePath) ? null : ImageSource.FromFile(ImagePath);

    [ObservableProperty]
    public partial double ImageScale { get; set; } = 1;

    [ObservableProperty]
    public partial double ImageXOffset { get; set; } = 0;

    [ObservableProperty]
    public partial double ImageYOffset { get; set; } = 0;

    [ObservableProperty]
    public partial bool IsColorPickerEnabled { get; set; }

    [RelayCommand]
    void ResetImageTransform()
    {
        ImageScale = 1;
        ImageXOffset = 0;
        ImageYOffset = 0;
    }

    [RelayCommand]
    async Task PickImage()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to extract colors"
        });
        if(result is not null)
        {
            ResetImageTransform();
            ImagePath = string.Empty;
            GC.Collect(); // Clear memory to avoid issues with large images
            ImagePath = result.FullPath;
            Colors.Clear();
        }
    }

    [RelayCommand]
    void ImageZoomIn()
    {
        ImageScale = Math.Min(ImageScale * 1.1, 5);
    }

    [RelayCommand]
    void ImageZoomOut()
    {
        ImageScale = Math.Max(ImageScale * 0.9, 1);
    }

    [RelayCommand]
    async Task ExtractColors()
    {
        if(string.IsNullOrWhiteSpace(ImagePath))
            return;
        var extractedColors = await ImagePaletteExtractor.ExtractAsync(ImagePath);
        foreach(var color in extractedColors)
            Colors.Add(new MiraiColorModel() { Name = $"{color.Percentage}%", Color = color.Color });
    }

    [RelayCommand]
    static void SelectColor(MiraiColorModel color)
    {
        if(color is null)
            return;
        color.IsSelected = !color.IsSelected;
    }

    [RelayCommand]
    async Task DeleteSelectedColors()
    {
        if(Colors.Count == 0)
            return;
        var selectedColors = Colors.Where(c => c.IsSelected);
        var isConfirm = await Shell.Current.DisplayAlert(StringResource.DeleteColor, string.Format(StringResource.RemoveColorDialogMessage, string.Join(";", selectedColors.Select(c => c.Name))), StringResource.Confirm, StringResource.Cancel);
        if(!isConfirm)
            return;
        foreach(var color in Colors.Where(c => c.IsSelected))
            Colors.Remove(color);
    }


}