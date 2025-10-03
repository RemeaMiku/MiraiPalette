using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace MiraiPalette.WinUI.Converters;

public partial class IsNullOrEmptyToVisibilityConverter : IValueConverter
{
    public static IsNullOrEmptyToVisibilityConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            null => Visibility.Collapsed,
            string str=> string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible,
            ICollection collection=> collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible,
            _ => Visibility.Visible,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}



[MarkupExtensionReturnType(ReturnType = typeof(IsNullOrEmptyToVisibilityConverter))]
public partial class IsNullOrEmptyToVisibilityConverterExtension : MarkupExtension
{
    public IsNullOrEmptyToVisibilityConverterExtension()
    {

    }

    protected override object ProvideValue()
        => IsNullOrEmptyToVisibilityConverter.Instance;
}