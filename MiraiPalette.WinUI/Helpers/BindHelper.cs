using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MiraiPalette.WinUI.Helpers;

public static class BindHelper
{
    public static bool InverseBool(bool value) => !value;

    public static Uri ToFormattedUri(string uriFormat, object args)
        => new(string.Format(uriFormat, args));

    public static ImageSource ToBitmapImage(Uri uri)
        => new BitmapImage(uri);
}
