using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PaletteDetailPage : Page
{
    public PaletteDetailPage()
    {
        InitializeComponent();
    }

    public PaletteDetailPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<PaletteDetailPageViewModel>();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var palette = e.Parameter as PaletteViewModel;
        ViewModel.LoadCommand.Execute(palette);
    }
}
