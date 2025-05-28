namespace MiraiPalette.Maui.AttachedProperties;

public class ButtonTextAlign
{
    public static readonly BindableProperty TextAlignProperty =
        BindableProperty.CreateAttached("TextAlign", typeof(TextAlignment), typeof(ButtonTextAlign), TextAlignment.Center,propertyChanged:OnTextAlignChanged);

    

    private static void OnTextAlignChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var button = bindable as Button ?? throw new ArgumentException("Bindable object must be a Button");
        var newTextAlign = (TextAlignment)newValue;
        if(button.Handler is null)
        {
            // If the handler is not set, we cannot apply the text alignment yet.
            // The alignment will be applied when the handler is set.
            button.HandlerChanged += (s, e) =>
            {
                if(button.Handler is null)
                    return;
                ApplyTextAlign(button.Handler.PlatformView, newTextAlign);
            };
            return;
        }
        ApplyTextAlign(button.Handler.PlatformView, newTextAlign);
    }

    private static void ApplyTextAlign(object? platformView, TextAlignment textAlign)
    {
#if WINDOWS
        var mauiButton = platformView as Microsoft.Maui.Platform.MauiButton ?? throw new InvalidOperationException("Button handler is not set or does not support MauiButton.");
        mauiButton.HorizontalContentAlignment = textAlign switch
        {
            TextAlignment.Start => Microsoft.UI.Xaml.HorizontalAlignment.Left,
            TextAlignment.Center => Microsoft.UI.Xaml.HorizontalAlignment.Center,
            TextAlignment.End => Microsoft.UI.Xaml.HorizontalAlignment.Right,
            _ => mauiButton.HorizontalContentAlignment,
        };
#endif
    }


    public static TextAlignment GetTextAlign(BindableObject view)
    {
        return (TextAlignment)view.GetValue(TextAlignProperty);
    }

    public static void SetTextAlign(BindableObject view, TextAlignment value)
    {
        view.SetValue(TextAlignProperty, value);
    }
}
