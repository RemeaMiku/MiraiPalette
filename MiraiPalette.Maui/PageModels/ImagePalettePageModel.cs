using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

public partial class ImagePalettePageModel : ObservableObject
{
    public ImagePalettePageModel(IPaletteRepositoryService paletteRepositoryService)
    {
        Colors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Colors));
        _paletteRepositoryService = paletteRepositoryService;
    }

    readonly IPaletteRepositoryService _paletteRepositoryService;

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

    public List<int> ColorCountOptions { get; } = Enumerable.Range(1, 16).ToList();

    [ObservableProperty]
    public partial int ColorCount { get; set; } = Constants.DefaultColorCount;

    [ObservableProperty]
    public partial bool IsBusy { get; set; } = false;

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
        if(IsBusy)
            return;
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to extract colors"
        });
        if(result is null)
            return;
        ResetImageTransform();
        ImagePath = string.Empty;
        GC.Collect(); // Clear memory to avoid issues with large images
        ImagePath = result.FullPath;
        Colors.Clear();
    }

    [RelayCommand]
    void ImageZoomIn()
    {
        if(string.IsNullOrWhiteSpace(ImagePath))
            return;
        ImageScale = Math.Min(ImageScale * 1.1, 5);
    }

    [RelayCommand]
    void ImageZoomOut()
    {
        if(string.IsNullOrWhiteSpace(ImagePath))
            return;
        ImageScale = Math.Max(ImageScale * 0.9, 1);
    }

    [RelayCommand]
    async Task ExtractColors()
    {
        if(IsBusy || string.IsNullOrWhiteSpace(ImagePath))
            return;
        Colors.Clear();
        IsBusy = true;
        foreach(var color in await ImagePaletteExtractor.ExtractAsync(ImagePath, ColorCount))
            Colors.Add(new MiraiColorModel() { Name = $"{color.Percentage:0.0}%", Color = color.Color });
        IsBusy = false;
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
        if(!selectedColors.Any())
            return;
        var isConfirm = await Shell.Current.DisplayAlert(StringResource.DeleteColor, string.Format(StringResource.RemoveColorDialogMessage, string.Join("\", \"", selectedColors.Select(c => $"{c.Hex}({c.Name})"))), StringResource.Confirm, StringResource.Cancel);
        if(!isConfirm)
            return;
        foreach(var color in selectedColors.ToArray())
            Colors.Remove(color);
    }

    [RelayCommand]
    async Task SaveColorsToPalette()
    {
        if(IsBusy || Colors.Count == 0)
            return;
        var palette = new MiraiPaletteModel()
        {
            Name = Path.GetFileNameWithoutExtension(ImagePath) ?? StringResource.NewPalette,
            Description = $"Extracted from {ImagePath}",
            Colors = new(Colors)
        };
        IsBusy = true;
        var paletteId = await _paletteRepositoryService.InsertPaletteAsync(palette);
        IsBusy = false;
        var args = new ShellNavigationQueryParameters
        {
            { nameof(MiraiPaletteModel.Id), paletteId }
        };
        // 先回到主页面，再跳转到详情页，避免绝对路由异常
        await Shell.Current.GoToAsync($"//{ShellRoutes.MainPage}");
        await Shell.Current.GoToAsync($"/{ShellRoutes.PaletteDetailPage}", args);
    }
}