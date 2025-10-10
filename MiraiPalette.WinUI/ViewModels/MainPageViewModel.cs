using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using MiraiPalette.WinUI.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel : PageViewModel
{
    public MainPageViewModel(IPaletteDataService paletteDataService)
    {
        PaletteDataService = paletteDataService;
        SelectedPalettes.CollectionChanged += (_, _)
            => OnPropertyChanged(nameof(HasSelectedPalettes));
    }

    [ObservableProperty]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    public IPaletteDataService PaletteDataService { get; }

    [ObservableProperty]
    public partial PaletteViewModel? CurrentPalette { get; set; }

    public ObservableCollection<PaletteViewModel> SelectedPalettes { get; } = [];

    public bool HasSelectedPalettes => SelectedPalettes.Count > 0;

    [ObservableProperty]
    public partial bool IsPalettePanelOpen { get; set; } = false;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [RelayCommand]
    async Task Load()
    {
        Palettes.Clear();
        SelectedPalettes.Clear();
        CurrentPalette = null;
        IsPalettePanelOpen = false;
        IsBusy = true;
        Palettes = new(await PaletteDataService.GetAllPalettesAsync());
        IsBusy = false;
    }

    partial void OnIsMultiSelectModeChanged(bool value)
    {
        if(value)
        {
            CurrentPalette = null;
            IsPalettePanelOpen = false;
        }
        else
        {
            foreach(var palette in Palettes)
            {
                palette.IsSelected = false;
            }
            SelectedPalettes.Clear();
        }
    }

    [RelayCommand]
    async Task DeleteSelectedPalettes()
    {
        IsBusy = true;
        await PaletteDataService.DeletePalettesAsync(SelectedPalettes.Select(p => p.Id));
        IsBusy = false;
        await Load();
    }

    [RelayCommand]
    void TogglePaletteSelection(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            DeselectPalette(palette);
        else
            SelectPalette(palette);
    }

    string GetNextColorName()
    {
        const string baseName = "新颜色";
        var names = CurrentPalette?.Colors.Select(c => c.Name).ToList() ?? [];
        if (!names.Contains(baseName))
            return baseName;

        var index = 2;
        while (names.Contains($"{baseName}{index}"))
            index++;
        return $"{baseName}{index}";
    }

    [RelayCommand]
    async Task AddColor()
    {
        if (CurrentPalette is null)
            throw new InvalidOperationException("No palette is selected.");
        var newColorModel = new ColorViewModel()
        {
            Name = GetNextColorName(),
            Color = Colors.White
        };
        CurrentPalette.Colors.Add(newColorModel);
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette);
        EditColor(newColorModel);
    }

    void SelectPalette(PaletteViewModel palette)
    {
        if(!IsMultiSelectMode)
        {
            CurrentPalette?.IsSelected = false;
            SelectedPalettes.Clear();
            CurrentPalette = palette;
            IsPalettePanelOpen = true;
        }
        palette.IsSelected = true;
        if(SelectedPalettes.Contains(palette))
            throw new InvalidOperationException("Palette is already selected.");
        SelectedPalettes.Add(palette);
    }

    void DeselectPalette(PaletteViewModel palette)
    {
        palette.IsSelected = false;
        SelectedPalettes.Remove(palette);
        if(!IsMultiSelectMode)
        {
            CurrentPalette = null;
            SelectedPalettes.Clear();
            IsPalettePanelOpen = false;
        }
    }

    partial void OnIsPalettePanelOpenChanged(bool value)
    {
        if(!value)
        {
            CurrentPalette?.IsSelected = false;
            CurrentPalette = null;
        }
    }

    [ObservableProperty]
    public partial Color PreviewColor { get; set; }

    [RelayCommand]
    void EditColor(ColorViewModel color)
    {
        SelectColor(color);
        PreviewColor = color.Color;
    }

    [RelayCommand]
    void SelectColor(ColorViewModel color)
    {
        foreach(var c in CurrentPalette!.Colors)
            c.IsSelected = false;
        color.IsSelected = true;
    }

    [RelayCommand]
    async Task SaveColor(ColorViewModel color)
    {
        if(CurrentPalette is null)
            throw new InvalidOperationException("No palette is selected.");
        color.Color = PreviewColor;
        IsBusy = true;
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette);
        IsBusy = false;
    }

    [RelayCommand]
    async Task DeleteSelectedColors()
    {
        if(CurrentPalette is null)
            throw new InvalidOperationException("No palette is selected.");
        IsBusy = true;
        for(int i = CurrentPalette.Colors.Count-1; i >= 0; i--)
        {
             if(CurrentPalette.Colors[i].IsSelected)
                 CurrentPalette.Colors.RemoveAt(i);
        }
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette);
        IsBusy = false;
    }

    [RelayCommand]
    void ResetPreviewColor(ColorViewModel color)
    {
        PreviewColor = color.Color;
    }

    [RelayCommand]
    static void CopyColorToClipboard(string colorValue)
    {
        var package = new DataPackage();
        package.SetText(colorValue);
        Clipboard.SetContent(package);
    }

    partial void OnCurrentPaletteChanged(PaletteViewModel? oldValue, PaletteViewModel? newValue)
    {
        if(oldValue is not null)
        {
            oldValue.PropertyChanged -= OnCurrentPalettePropertyChanged;
            foreach(var color in oldValue.Colors)
            {
                color.PropertyChanged -= OnCurrentPaletteColorPropertyChanged;
                color.IsSelected = false;
            }
        }
        if(newValue is not null)
        {
            newValue.PropertyChanged += OnCurrentPalettePropertyChanged;
            foreach(var color in newValue.Colors)
            {
                color.PropertyChanged += OnCurrentPaletteColorPropertyChanged;
            }
        }
    }

    private async void OnCurrentPaletteColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette!);
    }

    private async void OnCurrentPalettePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(PaletteViewModel.Title) && string.IsNullOrWhiteSpace(CurrentPalette!.Title))
        {
            CurrentPalette.Title = (await PaletteDataService.GetPaletteAsync(CurrentPalette.Id))!.Title;
            return;
        }
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette!);
    }
}
