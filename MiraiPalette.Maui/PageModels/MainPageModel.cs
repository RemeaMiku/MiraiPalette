using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Services;
using MiraiPalette.Maui.Utilities;

namespace MiraiPalette.Maui.PageModels;

public partial class MainPageModel(IPaletteRepositoryService paletteRepositoryService) : ObservableObject
{
    private readonly IPaletteRepositoryService _paletteRepositoryService = paletteRepositoryService;

    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    [ObservableProperty]
    public partial List<MiraiPaletteModel>? Palettes { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PaletteItemTapCommand))]
    public partial bool IsSelectionEnabled { get; set; } = false;

    partial void OnIsSelectionEnabledChanged(bool value)
    {
        if(!value)
            ClearSelection();
    }

    private void ClearSelection()
    {
        foreach(var palette in SelectedPalettes)
            palette.IsSelected = false;
        SelectedPalettes.Clear();
    }

    [RelayCommand]
    private void ToggleSelectionMode()
    {
        IsSelectionEnabled = !IsSelectionEnabled;
    }

    public IRelayCommand PaletteItemTapCommand => IsSelectionEnabled ? SelectPaletteCommand : NavigateToPaletteCommand;

    public ObservableCollection<MiraiPaletteModel> SelectedPalettes { get; set; } = [];

    [RelayCommand]
    private async Task Load()
    {
        Palettes = default;
        Palettes = await _paletteRepositoryService.ListPalettesAsync();
    }

    [RelayCommand]
    private void SelectPalette(MiraiPaletteModel palette)
    {
        palette.IsSelected = !palette.IsSelected;
        if(palette.IsSelected)
            SelectedPalettes.Add(palette);
        else
            SelectedPalettes.Remove(palette);
    }

    [RelayCommand]
    private static async Task NavigateToPalette(MiraiPaletteModel palette)
    {
        var args = new ShellNavigationQueryParameters
        {
            { nameof(MiraiPaletteModel.Id), palette.Id }
        };
        await Shell.Current.GoToAsync(ShellRoutes.PaletteDetailPage, args);
    }

    [RelayCommand]
    private async Task AddNewPalette()
    {
        IsSelectionEnabled = false;
        var palette = new MiraiPaletteModel { Name = Constants.DefaultPaletteName };
        palette.Id = await _paletteRepositoryService.InsertPaletteAsync(palette);
        await Load();
        await NavigateToPalette(palette);
    }

    [RelayCommand]
    private async Task DeletePalettes()
    {
        if(SelectedPalettes.Count == 0)
            return;
        var message = SelectedPalettes.Count == 1 ? $"Are you sure you want to delete \"{SelectedPalettes.First().Name}\"?" : $"Are you sure you want to delete these {SelectedPalettes.Count} palettes?{Environment.NewLine}{string.Join(Environment.NewLine, SelectedPalettes.Select(p => p.Name))}";
        var isYes = await Shell.Current.DisplayAlert("Delete Palette", message, "Yes", "No");
        if(!isYes)
            return;
        foreach(var palette in SelectedPalettes)
            await _paletteRepositoryService.DeletePaletteAsync(palette.Id);
        IsSelectionEnabled = false;
        await Load();
    }
}