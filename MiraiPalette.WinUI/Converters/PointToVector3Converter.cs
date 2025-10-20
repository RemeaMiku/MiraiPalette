using System;
using System.Numerics;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

namespace MiraiPalette.WinUI.Converters;

public partial class PointToVector3Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is Point point
            ? new Vector3((float)point.X, (float)point.Y, 0f)
            : new Vector3(0f, 0f, 0f);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
