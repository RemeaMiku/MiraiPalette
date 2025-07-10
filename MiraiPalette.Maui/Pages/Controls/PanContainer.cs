namespace MiraiPalette.Maui.Pages.Controls;

public partial class PanContainer : ContentView
{
    double _currentScale = 1;
    double _startScale = 1;
    double _xOffset = 0;
    double _yOffset = 0;

    public PanContainer()
    {
        var panGestureReconizer = new PanGestureRecognizer();
        var pinchGestureReconizer = new PinchGestureRecognizer();
        panGestureReconizer.PanUpdated += OnPanUpdated;
        pinchGestureReconizer.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(panGestureReconizer);
        GestureRecognizers.Add(pinchGestureReconizer);
#if WINDOWS
        // 监听鼠标滚轮
        HandlerChanged += (s, e) =>
        {
            if(Handler?.PlatformView is Microsoft.Maui.Platform.ContentPanel platformView)
            {
                platformView.PointerWheelChanged += NativeImage_PointerWheelChanged;
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

    void OnPinchUpdated(object? _, PinchGestureUpdatedEventArgs e)
    {
        if(e.Status == GestureStatus.Started)
        {
            _startScale = Content.Scale;
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;
        }
        else if(e.Status == GestureStatus.Running)
        {
            _currentScale = Math.Clamp(_startScale * e.Scale, 1, 5);
            Content.Scale = _currentScale;
        }
    }

    void OnPanUpdated(object? _, PanUpdatedEventArgs e)
    {
        switch(e.StatusType)
        {
            case GestureStatus.Running:
                // 计算拖动偏移
                double newX = _xOffset + e.TotalX;
                double newY = _yOffset + e.TotalY;
                var borderWidth = Width;
                var borderHeight = Height;
                var imgWidth = Content.Width * Content.Scale;
                var imgHeight = Content.Height * Content.Scale;
                var absMaxXOffset = Math.Abs(imgWidth - borderWidth) / 2;
                var absMaxYOffset = Math.Abs(imgHeight - borderHeight) / 2;
                Content.TranslationX = Math.Clamp(newX, -absMaxXOffset, absMaxXOffset);
                Content.TranslationY = Math.Clamp(newY, -absMaxYOffset, absMaxYOffset);
                break;
            case GestureStatus.Completed:
                // 记录最终偏移
                _xOffset = Content.TranslationX;
                _yOffset = Content.TranslationY;
                break;
        }
    }

    // 鼠标滚轮缩放时的缩放逻辑
    void ApplyScale(double scaleDelta, double centerX, double centerY)
    {
        var oldScale = Content.Scale;
        var newScale = Math.Max(1, oldScale * scaleDelta);
        Content.Scale = newScale;
        _currentScale = newScale;
        // 可根据需要调整锚点和偏移
    }
}