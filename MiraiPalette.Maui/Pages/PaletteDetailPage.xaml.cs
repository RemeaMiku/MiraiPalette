using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class PaletteDetailPage : ContentPage
{
    public PaletteDetailPage(PaletteDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}

