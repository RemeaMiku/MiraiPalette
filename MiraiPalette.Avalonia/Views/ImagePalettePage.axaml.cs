using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MiraiPalette.Avalonia.ViewModels;

namespace MiraiPalette.Avalonia.Views;

public partial class ImagePalettePage : UserControl
{
    public ImagePalettePage()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<ImagePalettePageViewModel>();
    }
}