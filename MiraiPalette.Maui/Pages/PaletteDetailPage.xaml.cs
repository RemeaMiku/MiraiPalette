using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class PaletteDetailPage : ContentPage
{
    public PaletteDetailPage(PaletteDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    Border? _lastBorder;

    private void ClearBorderStroke()
    {
        if(_lastBorder is not null)
        {
            _lastBorder.StrokeThickness = 0;
            _lastBorder.Padding = 1;
        }
    }

    private void ColorItemTapped(object sender, TappedEventArgs e)
    {
        var border = (Border)sender;
        if(border == _lastBorder)
            return;
        ClearBorderStroke();
        border.StrokeThickness = 2;
        border.Padding = 0;
        _lastBorder = border;
    }

    private void CloseColorDetailButtonClicked(object sender, EventArgs e)
    {
        ClearBorderStroke();
    }
}

