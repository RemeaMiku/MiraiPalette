using System.Globalization;

namespace MiraiPalette.Maui.Essentials;

public static class ColorUtilities
{
    private static bool TryParseInt(ReadOnlySpan<char> span, out int value)
    {
        return int.TryParse(span, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryParseRgb(ReadOnlySpan<char> hex, out Color color)
    {
        color = default!;
        if(hex.IsEmpty)
            return false;
        if(hex[0] == '#')
            hex = hex[1..];
        int r, g, b;
        if(hex.Length == 6)
        {
            if(!TryParseInt(hex[0..2], out r) || !TryParseInt(hex[2..4], out g) || !TryParseInt(hex[4..6], out b))
                return false;
        }
        //else if(hex.Length == 3)
        //{
        //    if(!TryParseInt(hex[0..1], out r) || !TryParseInt(hex[1..2], out g) || !TryParseInt(hex[2..3], out b))
        //        return false;
        //    r *= 17;
        //    g *= 17;
        //    b *= 17;
        //}
        else
            return false;
        if(r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            return false;
        color = Color.FromRgb(r, g, b);
        return true;
    }
}
