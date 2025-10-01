using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    public ObservableCollection<ColorViewModel> Colors { get; set; } = [];

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public PaletteViewModel()
    {
        
    }

    public PaletteViewModel(PaletteEntity entity)
    {
        Id = entity.Id;
        Title = entity.Name;
        Description = entity.Description;
        Colors = new ObservableCollection<ColorViewModel>(entity.Colors.Select(c => new ColorViewModel(c)));
    }
}