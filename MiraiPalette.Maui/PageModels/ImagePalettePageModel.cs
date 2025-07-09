using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.PageModels;

public partial class ImagePalettePageModel : ObservableObject
{
    public ObservableCollection<MiraiColorModel> Colors { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageSource))]
    public partial string ImagePath { get; set; } = string.Empty;

    public ImageSource? ImageSource => string.IsNullOrWhiteSpace(ImagePath) ? null : ImageSource.FromFile(ImagePath);

    [RelayCommand]
    async Task PickImage()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to extract colors"
        });
        if(result is not null)
        {
            ImagePath = result.FullPath;
            Colors.Clear();
            var colors = await ImagePaletteExtractor.ExtractAsync(result.FullPath);
            foreach(var color in colors)
            {
                Colors.Add(new MiraiColorModel() { Name = $"{color.Percentage}%", Color = color.Color });
            }
        }
    }

    [RelayCommand]
    static void SelectColor(MiraiColorModel color)
    {
        if(color is null)
            return;
        color.IsSelected = !color.IsSelected;
    }

    [RelayCommand]
    void DeleteSelectedColors()
    {
        foreach(var color in Colors.Where(c => c.IsSelected))
            Colors.Remove(color);
    }


}