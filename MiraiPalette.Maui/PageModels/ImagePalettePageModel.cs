using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

public partial class ImagePalettePageModel : ObservableObject, IQueryAttributable
{
    public ImagePalettePageModel(IPaletteService paletteRepositoryService)
    {
        Colors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Colors));
        _paletteRepositoryService = paletteRepositoryService;
    }

    private readonly IPaletteService _paletteRepositoryService;

    public ObservableCollection<MiraiColorModel> Colors { get; } = [];

    [ObservableProperty]
    public partial string ImagePath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ImageSource? ImageSource { get; set; }

    [ObservableProperty]
    public partial bool IsColorPickerEnabled { get; set; } = false;

    public List<int> ColorCountOptions { get; } =
    [
        Constants.DefaultColorCount,
        Constants.DefaultColorCount * 2,
        Constants.DefaultColorCount * 3,
        Constants.DefaultColorCount * 4
    ];

    [ObservableProperty]
    public partial int ColorCount { get; set; } = Constants.DefaultColorCount;

    [ObservableProperty]
    public partial Color? PickedColor { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; } = false;

    [RelayCommand]
    private async Task PickImage()
    {
        if(IsBusy)
            return;
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to extract colors"
        });
        if(result is null)
            return;
        ImagePath = string.Empty;
        GC.Collect(); // Clear memory to avoid issues with large images
        ImagePath = result.FullPath;
        ImageSource = ImageSource.FromFile(result.FullPath);
        Colors.Clear();
    }

    [RelayCommand]
    private void ToggleColorPicker()
    {
        IsColorPickerEnabled = !IsColorPickerEnabled;
        PickedColor = null;
    }

    [RelayCommand]
    private void SubmitPickedColor()
    {
        if(PickedColor is null)
            return;
        Colors.Add(new() { Name = $"{PickedColor.ToHex()}", Color = PickedColor });
        PickedColor = null;
    }

    [RelayCommand]
    private async Task ExtractColors()
    {
        if(IsBusy || string.IsNullOrWhiteSpace(ImagePath))
            return;
        for(int i = Colors.Count - 1; i >= 0; i--)
        {
            if(!Colors[i].Name.StartsWith('#'))
                Colors.Remove(Colors[i]);
        }
        IsBusy = true;
        foreach(var color in await ImagePaletteExtractor.ExtractAsync(ImagePath, ColorCount))
            Colors.Add(new MiraiColorModel() { Name = $"{color.Percentage:0.0}%", Color = color.Color });
        IsBusy = false;
    }

    [RelayCommand]
    private static void SelectColor(MiraiColorModel color)
    {
        if(color is null)
            return;
        color.IsSelected = !color.IsSelected;
    }

    [RelayCommand]
    private async Task DeleteSelectedColors()
    {
        if(Colors.Count == 0)
            return;
        var selectedColors = Colors.Where(c => c.IsSelected);
        if(!selectedColors.Any())
            return;
        var isConfirm = await Shell.Current.DisplayAlert(StringResource.DeleteColor, string.Format(StringResource.RemoveColorDialogMessage, string.Join("\"; \"", selectedColors.Select(c => c.Hex))), StringResource.Confirm, StringResource.Cancel);
        if(!isConfirm)
            return;
        foreach(var color in selectedColors.ToArray())
            Colors.Remove(color);
    }

    [RelayCommand]
    private async Task SaveColorsToPalette()
    {
        if(IsBusy || Colors.Count == 0)
            return;
        var palette = new MiraiPaletteModel()
        {
            Name = Path.GetFileNameWithoutExtension(ImagePath) ?? StringResource.NewPalette,
            Description = string.Format(StringResource.ExtractedFrom, ImagePath),
            Colors = new(Colors)
        };
        IsBusy = true;
        var paletteId = await _paletteRepositoryService.InsertPaletteAsync(palette);
        IsBusy = false;
        var args = new ShellNavigationQueryParameters
        {
            { nameof(MiraiPaletteModel.Id), paletteId }
        };
        await Shell.Current.GoToAsync(ShellRoutes.GoBack);
        await Shell.Current.GoToAsync($"{ShellRoutes.PaletteDetailPage}", args);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var imagePath = query[nameof(ImagePath)];
        if(imagePath is string path && !string.IsNullOrWhiteSpace(path))
        {
            ImagePath = path;
            ImageSource = ImageSource.FromFile(path);
        }
    }
}