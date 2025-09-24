using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; }

    public ObservableCollection<ColorViewModel> Colors { get; set; } = [];

}