using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

#pragma warning disable MVVMTK0045

public partial class MainPageModel : ObservableObject
{
    public MainPageModel(IPaletteRepositoryService paletteRepositoryService)
    {
        _paletteRepositoryService = paletteRepositoryService;
    }

    private readonly IPaletteRepositoryService _paletteRepositoryService;

    [ObservableProperty]
    private string _title = "Mirai Palette";

    [ObservableProperty]
    private List<Palette> _palettes = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PaletteItemTapCommand))]
    private bool _isSelectionEnabled = false;

    partial void OnIsSelectionEnabledChanged(bool value)
    {
        if(!value)
            ClearSelection();
    }

    private void ClearSelection()
    {
        foreach(var palette in SelectedPalettes)
        {
            palette.IsSelected = false;
        }
        SelectedPalettes.Clear();
        Palettes = [.. Palettes];
    }

    [RelayCommand]
    private void ToggleSelectionMode()
    {
        IsSelectionEnabled = !IsSelectionEnabled;
    }

    public IRelayCommand PaletteItemTapCommand => IsSelectionEnabled ? SelectPaletteCommand : NavigateToPaletteCommand;

    public ObservableCollection<Palette> SelectedPalettes { get; set; } = [];

    private async Task LoadAsync()
    {
        Palettes = default!;
        Palettes = await _paletteRepositoryService.ListAsync();
    }

    [RelayCommand]
    private async Task Appearing()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private void SelectPalette(Palette palette)
    {
        palette.IsSelected = !palette.IsSelected;
        if(palette.IsSelected)
            SelectedPalettes.Add(palette);
        else
            SelectedPalettes.Remove(palette);
        Palettes = [.. Palettes];
    }

    [RelayCommand]
    private static async Task NavigateToPalette(Palette palette)
    {
        var args = new ShellNavigationQueryParameters
        {
            { nameof(Palette.Id), palette.Id }
        };
        try
        {
            await Shell.Current.GoToAsync(ShellRoutes.PaletteDetailPage, args);
        }
        catch(Exception e)
        {
            await Shell.Current.DisplayAlert("", e.Message + Environment.NewLine + e.StackTrace, "OK");
        }

    }

    [RelayCommand]
    private async Task AddNewPalette()
    {
        IsSelectionEnabled = false;
        await Shell.Current.GoToAsync(ShellRoutes.PaletteDetailPage);
    }

    [RelayCommand]
    private async Task DeletePalettes()
    {
        if(SelectedPalettes.Count == 0)
            return;
        var message = SelectedPalettes.Count == 1 ? $"Are you sure you want to delete \"{SelectedPalettes.First().Name}\"?" : $"Are you sure you want to delete these {SelectedPalettes.Count} palettes?{Environment.NewLine}{string.Join(Environment.NewLine, SelectedPalettes.Select(p => p.Name))}";
        await Shell.Current.DisplayAlert("Delete Palette", message, "Yes", "No").ContinueWith(async task =>
        {
            if(task.Result)
            {
                foreach(var palette in SelectedPalettes)
                {
                    await _paletteRepositoryService.DeleteAsync(palette.Id);
                }
                await LoadAsync();
                IsSelectionEnabled = false;
            }
        });
    }
}