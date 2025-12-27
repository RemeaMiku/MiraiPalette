using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MiraiPalette.WinUI.Helpers;

public static class BindHelper
{
    public static bool InverseBool(bool value) => !value;

    public static Uri ToFormattedUri(string uriFormat, object args) => new(string.Format(uriFormat, args));

    public static ImageSource ToBitmapImage(Uri uri) => new BitmapImage(uri);

    public static Visibility NullableToVisibility(object? obj) => obj is null ? Visibility.Collapsed : Visibility.Visible;

    public static bool IsNotNullOrEmpty(object? obj) => obj switch
    {
        null => false,
        string str => !string.IsNullOrEmpty(str),
        ICollection collection => collection.Count > 0,
        _ => true,
    };

    public static bool IsNotNullOrEmpty<T>(IEnumerable<T> enumerable) => enumerable is not null && enumerable.Any();

    public static Visibility IsNotEmptyOrNullToVisibility(object? obj) => IsNotNullOrEmpty(obj) ? Visibility.Visible : Visibility.Collapsed;
}
