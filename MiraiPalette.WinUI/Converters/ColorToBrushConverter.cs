using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MiraiPalette.WinUI.Converters;

public partial class ColorToBrushConverter : IValueConverter
{
    public static ColorToBrushConverter Instance { get; } = new();

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return value is Color color ? new SolidColorBrush(color) : (object?)null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}


[MarkupExtensionReturnType(ReturnType = typeof(ColorToBrushConverter))]
public partial class ColorToBrushConverterExtension : MarkupExtension
{
    public ColorToBrushConverterExtension()
    {

    }

    protected override object ProvideValue()
        => ColorToBrushConverter.Instance;
}