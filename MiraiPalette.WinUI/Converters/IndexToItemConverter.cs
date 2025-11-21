using System;
using System.Collections;
using Microsoft.UI.Xaml.Data;

namespace MiraiPalette.WinUI.Converters;

public partial class IndexToItemConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if(value is null || parameter is not IEnumerable items)
            return -1;
        int index = 0;
        foreach(var item in items)
        {
            if(Equals(item, value))
                return index;
            index++;
        }
        return -1;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return parameter is not IList list ? default : value is int index && index >= 0 && index < list.Count ? list[index] : default;
    }
}
