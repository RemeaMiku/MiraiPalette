using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using MiraiPalette.WinUI.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel(IPaletteDataService paletteDataService) : PageViewModel
{
    [ObservableProperty]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    public IPaletteDataService PaletteDataService { get; } = paletteDataService;

    [ObservableProperty]
    public partial PaletteViewModel? CurrentPalette { get; set; }

    public ObservableCollection<PaletteViewModel> SelectedPalettes { get; } = [];

    [ObservableProperty]
    public partial bool IsPalettePanelOpen { get; set; } = false;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [RelayCommand]
    async Task Load()
    {
        IsBusy = true;
        Palettes.Clear();
        SelectedPalettes.Clear();
        CurrentPalette = null;
        Palettes = new(await PaletteDataService.GetAllPalettesAsync());
        IsBusy = false;
    }

    [RelayCommand]
    async Task DeleteSelectedPalettes()
    {
        IsBusy = true;
        await PaletteDataService.DeletePalettesAsync(SelectedPalettes.Select(p => p.Id));
        await Load();
        IsBusy = false;
    }

    [RelayCommand]
    void TogglePaletteSelection(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            DeselectPalette(palette);
        else
            SelectPalette(palette);
    }

    void SelectPalette(PaletteViewModel palette)
    {
        palette.IsSelected = true;
        if(SelectedPalettes.Contains(palette))
            throw new InvalidOperationException("Palette is already selected.");
        SelectedPalettes.Add(palette);
        if(!IsMultiSelectMode)
        {
            CurrentPalette?.IsSelected = false;
            SelectedPalettes.Clear();
            CurrentPalette = palette;
            IsPalettePanelOpen = true;
        }
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
    public partial Color PreviewColor { get; set; } = "#39c5bb".ToColor();

    [RelayCommand]
    void EditColor(ColorViewModel color)
    {
        foreach(var c in CurrentPalette!.Colors)
            c.IsSelected = false;
        PreviewColor = color.Color;
        color.IsSelected = true;
    }

    [RelayCommand]
    async Task SaveColor(ColorViewModel color)
    {
        if(CurrentPalette is null)
            return;
        color.Color = PreviewColor;
        IsBusy = true;
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
        }
        await PaletteDataService.UpdatePaletteAsync(CurrentPalette!);
    }
}
