using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
        PaletteCommand = NavigateToPaletteCommand;
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

    [ObservableProperty]
    public partial PaletteViewModel CurrentPalette { get; set; } = null!;

    public bool HasSelectedPalettes => SelectedPalettes.Count > 0;

    public bool IsAllPalettesSelected => Palettes.Count > 0 && SelectedPalettes.Count == Palettes.Count;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [ObservableProperty]
    public partial ICommand PaletteCommand { get; set; }

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
        if(value == true)
        {
            PaletteCommand = TogglePaletteSelectionCommand;
        }
        else
        {
            PaletteCommand = NavigateToPaletteCommand;
            ClearSelection();
        }
    }

    [RelayCommand]
    void SetCurrentPalette(PaletteViewModel palette)
    {
        CurrentPalette = palette;
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedPalettes(PaletteViewModel? palette)
    {
        if(palette is null || palette.IsSelected)
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
        }
        else
        {
            var confirmed = await Current.ShowConfirmDialogAsync("删除调色板", $"确定要删除调色板 \"{palette.Title}\" 吗？");
            if(!confirmed)
                return;
            IsBusy = true;
            await PaletteDataService.DeletePaletteAsync(palette.Id);
            IsBusy = false;
        }
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
    void NavigateToPalette(PaletteViewModel palette)
    {
        Current.NavigateTo(NavigationTarget.Palette, palette);
        ClearSelection();
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
        NavigateToPalette(newPalette);
    }

    [RelayCommand]
    void SelectPalette(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            throw new InvalidOperationException("Palette is already selected.");
        palette.IsSelected = true;
        SelectedPalettes.Add(palette);
    }

    [RelayCommand]
    void DeselectPalette(PaletteViewModel palette)
    {
        if(!palette.IsSelected)
            throw new InvalidOperationException("Palette is not selected.");
        palette.IsSelected = false;
        SelectedPalettes.Remove(palette);
    }

    [RelayCommand]
    async Task NavigateToImagePalette()
    {
        var path = await Current.PickFile("打开", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff");
        if(path is not null)
            Current.NavigateTo(NavigationTarget.ImagePalette, path);
        ClearSelection();
    }
}
