using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MiraiPalette.Avalonia.ViewModels;

namespace MiraiPalette.Avalonia.Views;

public partial class PalettesPage : UserControl
{
    public PalettesPage()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<PalettesPageViewModel>();
    }
}