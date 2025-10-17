using Windows.UI;

namespace MiraiPalette.WinUI.Essentials;

public static class ColorExtensions
{
    extension(Color color)
    {
        public string ToHex(bool includeAlpha = false)
            => includeAlpha
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
