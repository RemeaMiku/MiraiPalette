using MiraiPalette.Maui.PageModels;
using SkiaSharp;

namespace MiraiPalette.Maui.Pages;

public partial class ImagePalettePage : ContentPage
{
    private SKBitmap? _bitmapCache;

    public ImagePalettePage(ImagePalettePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadBitmapCache();
    }

    private void LoadBitmapCache()
    {
        var model = BindingContext as ImagePalettePageModel;
        if(model?.ImagePath is { Length: > 0 } path && File.Exists(path))
        {
            _bitmapCache?.Dispose();
            using var stream = File.OpenRead(path);
            _bitmapCache = SKBitmap.Decode(stream);
        }
    }

    private void MainImage_Tapped(object sender, TappedEventArgs e)
    {
        var model = BindingContext as ImagePalettePageModel;
        if(model?.ImagePath is { Length: > 0 } path && File.Exists(path))
        {
            _bitmapCache?.Dispose();
            using var stream = File.OpenRead(path);
            _bitmapCache = SKBitmap.Decode(stream);
        }

        // 获取点击点在Image控件内的坐标
        var touchPoint = e.GetPosition(MainImage);
        if(touchPoint is not Point point)
            return;

        // 获取Image控件实际显示区域
        var imageWidth = MainImage.Width;
        var imageHeight = MainImage.Height;
        var bmpWidth = _bitmapCache.Width;
        var bmpHeight = _bitmapCache.Height;

        // 计算图片在控件中的缩放和偏移（AspectFit）
        double scale = Math.Min(imageWidth / bmpWidth, imageHeight / bmpHeight);
        double displayW = bmpWidth * scale;
        double displayH = bmpHeight * scale;
        double offsetX = (imageWidth - displayW) / 2;
        double offsetY = (imageHeight - displayH) / 2;

        // 映射控件坐标到图片像素
        double imgX = (point.X - offsetX) / scale;
        double imgY = (point.Y - offsetY) / scale;

        int px = (int)Math.Clamp(imgX, 0, bmpWidth - 1);
        int py = (int)Math.Clamp(imgY, 0, bmpHeight - 1);

        var color = _bitmapCache.GetPixel(px, py);
        var mauiColor = Color.FromRgb(color.Red, color.Green, color.Blue);

        // 设置Rectangle的Fill
        ColorPreviewRect.Fill = new SolidColorBrush(mauiColor);
    }

    private void PointerGestureRecognizer_PointerMoved(object sender, PointerEventArgs e)
    {
        var model = BindingContext as ImagePalettePageModel;
        if(model?.ImagePath is { Length: > 0 } path && File.Exists(path))
        {
            _bitmapCache?.Dispose();
            using var stream = File.OpenRead(path);
            _bitmapCache = SKBitmap.Decode(stream);
        }

        // 获取点击点在Image控件内的坐标
        var touchPoint = e.GetPosition(MainImage);
        if(touchPoint is not Point point)
            return;

        // 获取Image控件实际显示区域
        var imageWidth = MainImage.Width;
        var imageHeight = MainImage.Height;
        var bmpWidth = _bitmapCache.Width;
        var bmpHeight = _bitmapCache.Height;

        // 计算图片在控件中的缩放和偏移（AspectFit）
        double scale = Math.Min(imageWidth / bmpWidth, imageHeight / bmpHeight);
        double displayW = bmpWidth * scale;
        double displayH = bmpHeight * scale;
        double offsetX = (imageWidth - displayW) / 2;
        double offsetY = (imageHeight - displayH) / 2;

        // 映射控件坐标到图片像素
        double imgX = (point.X - offsetX) / scale;
        double imgY = (point.Y - offsetY) / scale;

        int px = (int)Math.Clamp(imgX, 0, bmpWidth - 1);
        int py = (int)Math.Clamp(imgY, 0, bmpHeight - 1);

        var color = _bitmapCache.GetPixel(px, py);
        var mauiColor = Color.FromRgb(color.Red, color.Green, color.Blue);

        // 设置Rectangle的Fill
        ColorPreviewRect.Fill = new SolidColorBrush(mauiColor);
    }
}