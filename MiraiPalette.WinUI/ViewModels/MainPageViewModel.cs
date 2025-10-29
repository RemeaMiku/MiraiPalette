using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel : PageViewModel
{
    public MainPageViewModel(IPaletteDataService paletteDataService)
    {
        PaletteDataService = paletteDataService;
        SelectedPalettes.CollectionChanged += OnSelectedPalettesChanged;
    }

    private void OnSelectedPalettesChanged(object? _, NotifyCollectionChangedEventArgs __)
    {
        OnPropertyChanged(nameof(HasSelectedPalettes));
        OnPropertyChanged(nameof(IsAllPalettesSelected));
    }

    [ObservableProperty]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    public IPaletteDataService PaletteDataService { get; }

    public ObservableCollection<PaletteViewModel> SelectedPalettes { get; } = [];

    public bool HasSelectedPalettes => SelectedPalettes.Count > 0;

    public bool IsAllPalettesSelected => Palettes.Count > 0 && SelectedPalettes.Count == Palettes.Count;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [RelayCommand]
    private void SelectAllPalettes()
    {
        if(!IsMultiSelectMode)
            throw new InvalidOperationException("Not in multi-select mode.");
        foreach(var palette in Palettes)
        {
            if(!palette.IsSelected)
            {
                palette.IsSelected = true;
                SelectedPalettes.Add(palette);
            }
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach(var palette in SelectedPalettes)
            palette.IsSelected = false;
        SelectedPalettes.Clear();
    }

    [RelayCommand]
    async Task Load()
    {
        Palettes.Clear();
        SelectedPalettes.Clear();
        IsBusy = true;
        Palettes = new(await PaletteDataService.GetAllPalettesAsync());
        IsBusy = false;
    }

    partial void OnIsMultiSelectModeChanged(bool value)
    {
        if(value)
        {
        }
        else
        {
            foreach(var palette in Palettes)
                palette.IsSelected = false;
            SelectedPalettes.Clear();
        }
    }

    [RelayCommand]
    async Task DeleteSelectedPalettes()
    {
        var message = SelectedPalettes.Count == 1 ?
            $"确定要删除调色板 \"{SelectedPalettes[0].Title}\" 吗？" :
            $"确定要删除所选的 {SelectedPalettes.Count} 个调色板吗？";
        var confirmed = await Current.ShowConfirmDialogAsync("删除调色板", message);
        if(!confirmed)
            return;
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

    [RelayCommand]
    async Task AddPalette()
    {
        IsMultiSelectMode = false;
        var newPalette = new PaletteViewModel()
        {
            Title = "新建调色板",
            Description = string.Empty,
            Colors = []
        };
        IsBusy = true;
        await PaletteDataService.AddPaletteAsync(newPalette);
        Palettes.Insert(0, newPalette);
        IsBusy = false;
        SelectPalette(newPalette);
    }

    void SelectPalette(PaletteViewModel palette)
    {
        if(!IsMultiSelectMode)
        {
            SelectedPalettes.Clear();
            Current.NavigateTo(NavigationTarget.Palette, palette);
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
            ;
            SelectedPalettes.Clear();
        }
    }

}
