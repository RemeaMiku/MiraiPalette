using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    public partial bool IsPalettePanelOpen { get; set; } = false;

    [RelayCommand]
    async Task Load()
    {
        IsBusy = true;
        Palettes = new(await PaletteDataService.GetAllPalettesAsync());
        IsBusy = false;
    }

    [RelayCommand]
    async Task DeletePalette()
    {

    }

    [RelayCommand]
    void TogglePaletteSelection(PaletteViewModel palette)
    {
        CurrentPalette?.IsSelected = false;
        if(CurrentPalette == palette)
        {
            CurrentPalette = null;
            IsPalettePanelOpen = false;
            return;
        }
        CurrentPalette = palette;
        CurrentPalette.IsSelected = true;
        IsPalettePanelOpen = true;
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
        PreviewColor = color.Color;
    }

    [RelayCommand]
    void SaveColor(ColorViewModel color)
    {
        if(CurrentPalette is null)
            return;
        color.Color = PreviewColor;
    }

    [RelayCommand]
    void ResetPreviewColor(ColorViewModel color)
    {
        PreviewColor = color.Color;
    }

    [RelayCommand]
    static void CopyColorToClipboard(string hex)
    {
        var package = new DataPackage();
        package.SetText(hex);
        Clipboard.SetContent(package);
    }
}
