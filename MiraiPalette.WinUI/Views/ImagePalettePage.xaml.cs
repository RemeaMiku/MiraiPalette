using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
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

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var path = (string)e.Parameter;
        SourceImage.Source = new BitmapImage(new(path));
        ViewModel.ImagePath = path;
        ViewModel.ImagePixels = await new ImagePixelsExtractor().ExtractImagePixelsAsync(path);
    }

    private float GetMinZoomFactorToFitImage()
    {
        var bitmapImage = (BitmapImage)SourceImage.Source;
        var factor = (float)Math.Min(ImageScrollView.ActualWidth / bitmapImage.PixelWidth, ImageScrollView.ActualHeight / bitmapImage.PixelHeight);
        /// 保证不会小于 0.1 倍
        return MathF.Max(0.1f, factor);
    }

    private void OnImage_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        ViewModel.PointerPositionOnImage = e.GetCurrentPoint(SourceImage).Position;
    }

    private void OnImage_ImageOpened(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var minZoomFactor = GetMinZoomFactorToFitImage();
        ImageScrollView.ZoomToFactor(minZoomFactor);
        ImageScrollView.MinZoomFactor = minZoomFactor;
    }

    private void ColorCountNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        => sender.Value = Math.Floor(sender.Value);

    private void FitImageSizeToPanelButton_Click(object _sender, RoutedEventArgs _e)
        => ImageScrollView.ZoomToFactor(GetMinZoomFactorToFitImage());

    private void ImageZoomInButton_Click(object _sender, RoutedEventArgs _e)
        => ImageScrollView.ZoomToFactor(ImageScrollView.ZoomFactor * 1.25f);

    private void ImageZoomOutButton_Click(object _sender, RoutedEventArgs _e)
        => ImageScrollView.ZoomToFactor(ImageScrollView.ZoomFactor * 0.8f);

    private void ImageScrollView_SizeChanged(object _, SizeChangedEventArgs __)
    {
        if(!SourceImage.IsLoaded)
            return;
        ImageScrollView.MinZoomFactor = GetMinZoomFactorToFitImage();
    }

}
