using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace MiraiPalette.WinUI.Converters;

public partial class BoolToListViewSelectionModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue ? boolValue ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single : ListViewSelectionMode.Single;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
