namespace MiraiPalette.Maui.Pages.Controls;

public partial class PanContainer : ContentView
{
    private double _xOffset = 0;
    private double _yOffset = 0;

    public bool IsPanEnabled
    {
        get => (bool)GetValue(IsPanEnabledProperty);
        set => SetValue(IsPanEnabledProperty, value);
    }

    public bool IsZoomEnabled
    {
        get => (bool)GetValue(IsZoomEnabledProperty);
        set => SetValue(IsZoomEnabledProperty, value);
    }

    public double MinScale
    {
        get => (double)GetValue(MinScaleProperty);
        set => SetValue(MinScaleProperty, value);
    }

    public double MaxScale
    {
        get => (double)GetValue(MaxScaleProperty);
        set => SetValue(MaxScaleProperty, value);
    }

    public static readonly BindableProperty IsZoomEnabledProperty =
        BindableProperty.Create(nameof(IsZoomEnabled), typeof(bool), typeof(PanContainer), true);

    public static readonly BindableProperty IsPanEnabledProperty =
        BindableProperty.Create(nameof(IsPanEnabled), typeof(bool), typeof(PanContainer), true);

    public static readonly BindableProperty MinScaleProperty = BindableProperty.Create(
        nameof(MinScale), typeof(double), typeof(PanContainer), 1d);

    public static readonly BindableProperty MaxScaleProperty = BindableProperty.Create(
        nameof(MaxScale), typeof(double), typeof(PanContainer), 5d);

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
        if(!IsZoomEnabled)
            return;
        var delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
        var scaleDelta = delta > 0 ? 1.1 : 0.9;
        ApplyScale(Content.Scale * scaleDelta);
    }
#endif

    private void OnPinchUpdated(object? _, PinchGestureUpdatedEventArgs e)
    {
        if(!IsZoomEnabled)
            return;
        if(e.Status == GestureStatus.Started)
        {
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;
        }
        else if(e.Status == GestureStatus.Running)
        {
            ApplyScale(e.Scale);
        }
    }

    private void OnPanUpdated(object? _, PanUpdatedEventArgs e)
    {
        if(!IsPanEnabled)
            return;
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
    public void ApplyScale(double scale)
    {
        Content.Scale = Math.Clamp(scale, MinScale, MaxScale);
    }

    public void ResetTransform()
    {
        Content.TranslationX = 0;
        Content.TranslationY = 0;
        Content.Scale = MinScale;
    }
}