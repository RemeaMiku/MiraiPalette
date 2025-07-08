using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class PaletteDetailPage : ContentPage
{
    public PaletteDetailPage(PaletteDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
        Application.Current!.RequestedThemeChanged += Current_RequestedThemeChanged;        
    }

    private readonly PaletteDetailPageModel _model;

    private void Current_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        _model.UnloadCommand.Execute(null);
        _model.LoadCommand.Execute(null);
    }
}

