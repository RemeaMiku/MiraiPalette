using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel(IPaletteDataService paletteDataService) : PageViewModel
{
    [ObservableProperty]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    public IPaletteDataService PaletteDataService { get; } = paletteDataService;

}
