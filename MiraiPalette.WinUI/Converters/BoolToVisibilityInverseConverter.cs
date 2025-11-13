using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MiraiPalette.WinUI.Converters;

public class BoolToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue ? boolValue ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
