using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class ImagePalettePage : ContentPage
{
    public ImagePalettePage(ImagePalettePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}