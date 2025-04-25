using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
        Application.Current!.RequestedThemeChanged += Current_RequestedThemeChanged;
    }

    private void Current_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if(!_model.IsSelectionEnabled)
        {

        }
    }

    private MainPageModel _model;


}

