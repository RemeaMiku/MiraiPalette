using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

public partial class PaletteDetailPageModel(IPaletteRepositoryService paletteRepositoryService) : ObservableObject, IQueryAttributable
{
    private void OnPalettePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is nameof(MiraiPaletteModel.Name) or nameof(MiraiPaletteModel.Description))
        {
            _paletteRepositoryService.UpdatePaletteAsync(Palette);
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        CloseColorDetail();
        await Shell.Current.GoToAsync(ShellRoutes.GoBack);
    }

    private readonly IPaletteRepositoryService _paletteRepositoryService = paletteRepositoryService;

    [ObservableProperty]
    public partial MiraiPaletteModel Palette { get; set; } = new()
    {
        Name = Constants.DefaultPaletteName,
    };

    partial void OnPaletteChanged(MiraiPaletteModel oldValue, MiraiPaletteModel newValue)
    {
        if(oldValue != null)
            oldValue.PropertyChanged -= OnPalettePropertyChanged;
        if(newValue != null)
            newValue.PropertyChanged += OnPalettePropertyChanged;
    }

    [ObservableProperty]
    public partial MiraiColorModel? SelectedExistColor { get; set; }

    [ObservableProperty]
    public partial bool IsColorDetailOpen { get; set; } = false;

    public MiraiColorModel CurrentColor { get; } = new()
    {
        Name = Constants.DefaultColorName,
        Color = Color.FromArgb(Constants.DefaultColorAsHex),
    };

    [RelayCommand]
    private void AddNewColor()
    {
        OpenColorDetail(default);
    }

    [RelayCommand]
    private async Task DeletePalette()
    {
        var isYes = await Shell.Current.DisplayAlert("Delete Palette", "Are you sure you want to delete this palette?", "Yes", "No");
        if(!isYes)
            return;
        await _paletteRepositoryService.DeletePaletteAsync(Palette.Id);
        await Shell.Current.GoToAsync(ShellRoutes.GoBack);
    }

    [RelayCommand]
    private async Task DeleteSelectedColor()
    {
        if(SelectedExistColor is null)
            throw new InvalidOperationException("Selected color is null");
        var isYes = await Shell.Current.DisplayAlert("Delete Color", "Are you sure you want to delete this color?", "Yes", "No");
        if(!isYes)
            return;
        await _paletteRepositoryService.DeleteColorAsync(SelectedExistColor.Id);
        await Load();
        CloseColorDetail();
    }

    private void ClearColorDetail()
    {
        CurrentColor.Name = Constants.DefaultColorName;
        CurrentColor.Color = Color.FromArgb(Constants.DefaultColorAsHex);
    }

    private void ClearSelection()
    {
        if(SelectedExistColor is not null)
        {
            SelectedExistColor.IsSelected = false;
            SelectedExistColor = default;
        }
    }

    [RelayCommand]
    private void OpenColorDetail(MiraiColorModel? color)
    {
        ClearSelection();
        SelectedExistColor = color;
        if(SelectedExistColor is null)
            ClearColorDetail();
        else
        {
            SelectedExistColor.IsSelected = true;
            CurrentColor.Name = SelectedExistColor.Name;
            CurrentColor.Color = SelectedExistColor.Color;
        }
        IsColorDetailOpen = true;
    }

    [RelayCommand]
    private void CloseColorDetail()
    {
        IsColorDetailOpen = false;
        ClearColorDetail();
        ClearSelection();
    }

    [RelayCommand]
    private async Task SaveColorDetail()
    {
        if(SelectedExistColor is null)
        {
            await _paletteRepositoryService.InsertColorAsync(Palette.Id, new MiraiColorModel
            {
                Name = CurrentColor.Name,
                Color = CurrentColor.Color,
            });
        }
        else
        {
            await _paletteRepositoryService.UpdateColorAsync(new MiraiColorModel
            {
                Id = SelectedExistColor.Id,
                Name = CurrentColor.Name,
                Color = CurrentColor.Color,
            });
        }
        CloseColorDetail();
        await Load();
    }

    private int _paletteId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _paletteId = query.TryGetValue(nameof(MiraiPaletteModel.Id), out var idObj) && idObj is int id
            ? id
            : throw new ArgumentException("Palette Id not found in query parameters", nameof(query));
    }

    [RelayCommand]
    private async Task Load()
    {
        ClearColorDetail();
        ClearSelection();
        var palette = await _paletteRepositoryService.SelectPaletteAsync(_paletteId);
        if(palette is null)
        {
            await Shell.Current.DisplayAlert("Error", "Palette not found", "OK");
            await Shell.Current.GoToAsync(ShellRoutes.GoBack);
            return;
        }
        Palette = palette;
    }
}
