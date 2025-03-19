using MauiIcons.Core;
using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        _ = new MauiIcon();
        BindingContext = model;
    }
}

