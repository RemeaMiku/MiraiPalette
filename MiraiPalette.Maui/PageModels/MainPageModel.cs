using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

public partial class MainPageModel(IPaletteRepositoryService paletteRepositoryService) : ObservableObject
{
    private readonly IPaletteRepositoryService _paletteRepositoryService = paletteRepositoryService;

    public string ToggleSelectionModeText { get; } = StringResource.ToggleSelectionMode;

    public string SelectAllText { get; } = StringResource.SelectAll;

    public string AddNewPaletteText { get; } = StringResource.AddNewPalette;

    public string DeletePalettesText { get; } = StringResource.DeletePalettes;

    [ObservableProperty]
    public partial string Title { get; set; } = StringResource.MainWindowTitle;

    [ObservableProperty]
    public partial List<MiraiPaletteModel>? Palettes { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PaletteItemTapCommand))]
    public partial bool IsSelectionEnabled { get; set; } = false;

    public bool IsAllSelected
    {
        get => SelectedPalettes.Count == Palettes?.Count;
        set
        {
            if(value == IsAllSelected)
                return;
            if(value)
                SelectAllPalettes();
            else
                ClearSelection();
        }
    }

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
        OnPropertyChanged(nameof(IsAllSelected));
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
        ClearSelection();
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
        OnPropertyChanged(nameof(IsAllSelected));
    }

    private void SelectAllPalettes()
    {
        if(Palettes is null)
            return;
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
        var message = SelectedPalettes.Count == 1 ? string.Format(StringResource.DeletePaletteDialogMessage, SelectedPalettes[0].Name) : $"{string.Format(StringResource.DeletePalettesDialogMessage, SelectedPalettes.Count)}{Environment.NewLine}{string.Join("; ", SelectedPalettes.Select(p => p.Name))}";
        var isYes = await Shell.Current.DisplayAlert(StringResource.DeletePalettes, message, StringResource.Confirm, StringResource.Cancel);
        if(!isYes)
            return;
        foreach(var palette in SelectedPalettes)
            await _paletteRepositoryService.DeletePaletteAsync(palette.Id);
        IsSelectionEnabled = false;
        await Load();
    }
}