using System;
using Microsoft.UI.Xaml.Data;

namespace MiraiPalette.WinUI.Converters;

public partial class SafeTypeConverter : IValueConverter
{
    public Type? SourceType { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value == null ? null : SourceType != null && !SourceType.IsInstanceOfType(value) ? null : value;
    }
}

