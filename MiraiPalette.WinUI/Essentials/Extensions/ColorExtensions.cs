using System.Text.RegularExpressions;
using Windows.UI;

namespace MiraiPalette.WinUI.Essentials;

public static partial class ColorExtensions
{
    extension(Color color)
    {
        public string ToHex(bool includeAlpha = false)
            => includeAlpha
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    extension(string input)
    {
        public bool IsValidHexColor()
        {
            if(string.IsNullOrWhiteSpace(input))
                return false;
            string s = input.Trim();
            if(s.StartsWith('#'))
                s = s[1..];
            // 仅允许 3,6,8 位且全部为 0-9 a-f A-F
            return s.Length is 3 or 6 or 8 && HexColorRegex().IsMatch(s);
        }
    }

    [GeneratedRegex(@"\A[0-9a-fA-F]+\z")]
    private static partial Regex HexColorRegex();
}
