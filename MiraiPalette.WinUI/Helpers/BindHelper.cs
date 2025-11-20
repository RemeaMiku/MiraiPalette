using System;

namespace MiraiPalette.WinUI.Helpers;

public static class BindHelper
{
    public static bool InverseBool(bool value) => !value;

    public static Uri ToFormattedUri(string uriFormat, object args)
        => new(string.Format(uriFormat, args));
}
