using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class ImagePalettePage : ContentPage
{
    double _currentScale = 1;
    double _startScale = 1;
    double _xOffset = 0;
    double _yOffset = 0;

    public ImagePalettePage(ImagePalettePageModel model)
    {
        InitializeComponent();
        BindingContext = model;

#if WINDOWS
        // 监听鼠标滚轮
        ZoomableImage.HandlerChanged += (s, e) =>
        {
            if(ZoomableImage.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.Image nativeImage)
            {
                nativeImage.PointerWheelChanged += NativeImage_PointerWheelChanged;
            }
        };
#endif
    }

#if WINDOWS
    private void NativeImage_PointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
        var scaleDelta = delta > 0 ? 1.1 : 0.9;
        ApplyScale(scaleDelta, e.GetCurrentPoint(null).Position.X, e.GetCurrentPoint(null).Position.Y);
    }
#endif

    void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if(e.Status == GestureStatus.Started)
        {
            _startScale = ZoomableImage.Scale;
            ZoomableImage.AnchorX = 0.5;
            ZoomableImage.AnchorY = 0.5;
        }
        else if(e.Status == GestureStatus.Running)
        {
            _currentScale = Math.Max(1, _startScale * e.Scale);
            ZoomableImage.Scale = _currentScale;
        }
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch(e.StatusType)
        {
            case GestureStatus.Running:
                // 计算拖动偏移
                double newX = _xOffset + e.TotalX;
                double newY = _yOffset + e.TotalY;

                // 限制图片在Border范围内
                var borderWidth = ImageBorder.Width;
                var borderHeight = ImageBorder.Height;
                var imgWidth = ZoomableImage.Width * ZoomableImage.Scale;
                var imgHeight = ZoomableImage.Height * ZoomableImage.Scale;

                // 只允许在边界内拖动
                double minX = Math.Min(0, borderWidth - imgWidth);
                double minY = Math.Min(0, borderHeight - imgHeight);
                double maxX = 0;
                double maxY = 0;

                ZoomableImage.TranslationX = Math.Min(maxX, Math.Max(minX, newX));
                ZoomableImage.TranslationY = Math.Min(maxY, Math.Max(minY, newY));
                break;
            case GestureStatus.Completed:
                // 记录最终偏移
                _xOffset = ZoomableImage.TranslationX;
                _yOffset = ZoomableImage.TranslationY;
                break;
        }
    }

    // 鼠标滚轮缩放时的缩放逻辑
    void ApplyScale(double scaleDelta, double centerX, double centerY)
    {
        var oldScale = ZoomableImage.Scale;
        var newScale = Math.Max(1, oldScale * scaleDelta);
        ZoomableImage.Scale = newScale;
        _currentScale = newScale;
        // 可根据需要调整锚点和偏移
    }
}