using System.ComponentModel;
using MiraiPalette.Maui.PageModels;
using SkiaSharp;

namespace MiraiPalette.Maui.Pages;

public partial class ImagePalettePage : ContentPage
{
    private SKBitmap? _bitmapCache;
    private readonly ImagePalettePageModel _viewModel;

    public ImagePalettePage(ImagePalettePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _viewModel = model;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ImagePalettePageModel.ImagePath))
        {
            LoadBitmapCache();
        }
    }

    private void LoadBitmapCache()
    {
        if(!string.IsNullOrEmpty(_viewModel.ImagePath) && File.Exists(_viewModel.ImagePath))
        {
            _bitmapCache?.Dispose();
            using var stream = File.OpenRead(_viewModel.ImagePath);
            _bitmapCache = SKBitmap.Decode(stream);
        }
    }

    private void OnImageViewPointerMoved(object sender, PointerEventArgs e)
    {
        if(_bitmapCache is null || !_isPointerPressed || !_viewModel.IsColorPickerEnabled)
            return;

        SetPositionForPickedColorPreview(e);

        // 获取点击点在Image控件内的坐标
        var touchPointOnImage = e.GetPosition(_imageView);
        if(!touchPointOnImage.HasValue)
            return;

        // 获取Image控件实际显示区域
        var imageWidth = _imageView.Width;
        var imageHeight = _imageView.Height;
        var bmpWidth = _bitmapCache.Width;
        var bmpHeight = _bitmapCache.Height;

        // 计算图片在控件中的缩放和偏移（AspectFit）
        double scale = Math.Min(imageWidth / bmpWidth, imageHeight / bmpHeight);
        double displayW = bmpWidth * scale;
        double displayH = bmpHeight * scale;
        double offsetX = (imageWidth - displayW) / 2;
        double offsetY = (imageHeight - displayH) / 2;

        // 映射控件坐标到图片像素
        double imgX = (touchPointOnImage.Value.X - offsetX) / scale;
        double imgY = (touchPointOnImage.Value.Y - offsetY) / scale;

        int px = (int)Math.Clamp(imgX, 0, bmpWidth - 1);
        int py = (int)Math.Clamp(imgY, 0, bmpHeight - 1);

        var skColor = _bitmapCache.GetPixel(px, py);
        var pickedColor = Color.FromRgb(skColor.Red, skColor.Green, skColor.Blue);

        // 设置PickedColor
        _viewModel.PickedColor = pickedColor;
    }

    bool _isPointerPressed = false;

    private void SetPositionForPickedColorPreview(PointerEventArgs e)
    {
        var touchPointOnContainer = e.GetPosition(_imageContainer);
        if(touchPointOnContainer.HasValue)
        {
            _pickedColorPreview.TranslationX = touchPointOnContainer.Value.X - _pickedColorPreview.Width / 2;
            _pickedColorPreview.TranslationY = touchPointOnContainer.Value.Y - _pickedColorPreview.Height / 2;
        }
    }

    private void OnImageViewPointerPressed(object sender, PointerEventArgs e)
    {
        if(_bitmapCache is null || !_viewModel.IsColorPickerEnabled)
            return;
        _isPointerPressed = true;
        _pickedColorPreview.IsVisible = true;
        SetPositionForPickedColorPreview(e);
    }

    private void OnImageViewPointerReleased(object sender, PointerEventArgs e)
    {
        if(_bitmapCache is null || !_viewModel.IsColorPickerEnabled)
            return;
        _isPointerPressed = false;
        _pickedColorPreview.IsVisible = false;
    }

    private void OnZoomInButtonTapped(object sender, TappedEventArgs e)
    {
        _imageContainer.ApplyScale(_imageView.Scale * 1.1);
    }

    private void OnZoomOutButtonTapped(object sender, TappedEventArgs e)
    {
        _imageContainer.ApplyScale(_imageView.Scale / 1.1);
    }

    private void OnResetTransformButtonTapped(object sender, TappedEventArgs e)
    {
        _imageContainer.ResetTransform();
    }
}