using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Services;
using MiraiPalette.Maui.Utilities;

#pragma warning disable MVVMTK0045

namespace MiraiPalette.Maui.PageModels;

public partial class PaletteDetailPageModel : ObservableObject, IQueryAttributable
{
    public PaletteDetailPageModel(IPaletteRepositoryService paletteRepositoryService)
    {
        _paletteRepositoryService = paletteRepositoryService;
    }

    private readonly IPaletteRepositoryService _paletteRepositoryService;

    [ObservableProperty]
    private string _name = Constans.DefaultColorName;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private List<MiraiColor> _colors = [];

    private int _id = 0;

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private MiraiColor? _selectedExistColor;

    [ObservableProperty]
    private bool _isColorDetailOpen = false;

    [ObservableProperty]
    private string _currentColorName = Constans.DefaultColorName;

    [ObservableProperty]
    private Color _currentColorValue = Color.FromArgb(Constans.DefaultColorAsHex);

    [RelayCommand]
    private void AddNewColor()
    {
        OpenColorDetail(default);
    }

    [RelayCommand]
    private async Task DeletePalette()
    {
        var deleteConfirmed = await Shell.Current.DisplayAlert("Delete Palette", "Are you sure you want to delete this palette?", "Yes", "No");
        if(!deleteConfirmed)
            return;
        await _paletteRepositoryService.DeleteAsync(_id);
        await Shell.Current.GoToAsync(ShellRoutes.GoBack);
    }

    [RelayCommand]
    private async Task DeleteSelectedColor()
    {
        if(SelectedExistColor is null)
            throw new InvalidOperationException("Selected color is null");
        var deleteConfirmed = await Shell.Current.DisplayAlert("Delete Color", "Are you sure you want to delete this color?", "Yes", "No");
        if(!deleteConfirmed)
            return;
        Colors.Remove(SelectedExistColor);
        await SaveAsync();
        CloseColorDetail();
        Load(_id);
    }

    private void ClearColorDetail()
    {
        CurrentColorName = Constans.DefaultColorName;
        CurrentColorValue = Color.FromArgb(Constans.DefaultColorAsHex);
    }

    [RelayCommand]
    private void OpenColorDetail(MiraiColor? color)
    {
        SelectedExistColor = color;
        if(color is null)
            ClearColorDetail();
        else
        {
            CurrentColorName = color.Name;
            CurrentColorValue = color.Color;
        }
        IsColorDetailOpen = true;
    }

    [RelayCommand]
    private void CloseColorDetail()
    {
        IsColorDetailOpen = false;
        ClearColorDetail();
        SelectedExistColor = default;
    }

    [RelayCommand]
    private async Task SaveColorDetail()
    {
        if(IsBusy)
            return;
        IsBusy = true;
        if(SelectedExistColor is null)
        {
            Colors.Add(new MiraiColor
            {
                Name = CurrentColorName,
                Color = CurrentColorValue
            });
        }
        else
        {
            SelectedExistColor.Name = CurrentColorName;
            SelectedExistColor.Color = CurrentColorValue;
        }
        CloseColorDetail();
        await SaveAsync();
        IsBusy = false;
        Load(_id);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if(query.TryGetValue(nameof(Palette.Id), out var idObj) && idObj is int id)
        {
            Load(id);
        }
        else
        {
            Name = Constans.DefaultPaletteName;
        }
    }

    public void Load(int id)
    {
        if(IsBusy)
            return;
        IsBusy = true;
        var palette = _paletteRepositoryService.GetAsync(id).Result;
        if(palette is null)
            return;
        _id = palette.Id;
        Name = palette.Name;
        Description = palette.Description;
        Colors = default!;
        Colors = palette.Colors;
        IsBusy = false;
    }

    private async Task SaveAsync()
    {
        _id = await _paletteRepositoryService.SaveAsync(new Palette
        {
            Id = _id,
            Name = Name,
            Description = Description,
            Colors = Colors
        });
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if(!IsBusy && (e.PropertyName is nameof(Name) or nameof(Description)))
        {
            SaveAsync();
        }
        base.OnPropertyChanged(e);
    }
}
