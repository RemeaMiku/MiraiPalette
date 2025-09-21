using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class ColorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial Color Color { get; set; }

    public SolidColorBrush Brush => new(Color);
}
