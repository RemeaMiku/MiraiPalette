using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ImagePalettePage : Page
{
    public ImagePalettePage()
    {
        InitializeComponent();
    }

    public ImagePalettePageViewModel ViewModel { get; } = Current.Services.GetRequiredService<ImagePalettePageViewModel>();
    ImagePixels _pixels = null!;

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var path = (string)e.Parameter;
        _sourceImage.Source = new BitmapImage(new(path));
        _pixels = await new ImagePixelsExtractor().ExtractImagePixelsAsync(path);
    }

    private void OnImage_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(_sourceImage).Position;
        //TODO 
    }
}
