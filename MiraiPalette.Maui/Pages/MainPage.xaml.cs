using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}

