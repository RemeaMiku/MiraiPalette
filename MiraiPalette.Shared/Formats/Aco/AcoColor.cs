using System.Globalization;

namespace MiraiPalette.Shared.Formats.Aco;

/// <summary>
/// Represents a color in an Adobe Color (ACO) file.
/// </summary>
public class AcoColor
{
    /// <summary>
    /// Color space value of RGB
    /// </summary>
    public const ushort ColorSpaceRgb = 0;
    /// <summary>
    /// Color space value of CMYK
    /// </summary>
    public const ushort ColorSpaceCmyk = 2;
    /// <summary>
    /// Color space value of Lab
    /// </summary>
    public const ushort ColorSpaceLab = 7;
    /// <summary>
    /// Color space value of Gray
    /// </summary>
    public const ushort ColorSpaceGray = 8;

    /// <summary>
    /// Color space identifier. 0=RGB, 2=CMYK, 7=Lab, 8=Gray
    /// </summary>
    public ushort ColorSpace { get; set; }

    /// <summary>
    /// Color components. The number and meaning of these components depends on the color space. Component values are in the range 0 to 65535.
    /// </summary>
    public ushort[] Components { get; set; } = new ushort[4];

    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Creates an AcoColor from RGB values (0..255)
    /// </summary>
    /// <param name="r">Red component (0..255)</param>
    /// <param name="g">Green component (0..255)</param>
    /// <param name="b">Blue component (0..255)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified RGB color</returns>
    public static AcoColor FromRgb(byte r, byte g, byte b, string name = "")
    {
        return new()
        {
            ColorSpace = ColorSpaceRgb,
            Components = [(ushort)(r * 257), (ushort)(g * 257), (ushort)(b * 257), 0],
            Name = name
        };
    }

    /// <summary>
    /// Creates an AcoColor from a packed RGB value (0xRRGGBB)
    /// </summary>
    /// <param name="rgb">Packed RGB value (0xRRGGBB)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified RGB color</returns>
    public static AcoColor FromRgb(uint rgb, string name = "")
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return FromRgb(r, g, b, name);
    }

    /// <summary>
    /// Creates an AcoColor from a hexadecimal color string (e.g. #RRGGBB)
    /// </summary>
    /// <param name="hex">Hexadecimal color string (e.g. #RRGGBB)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified RGB color</returns>
    public static AcoColor FromHex(string hex, string name = "")
    {
        hex = hex.TrimStart('#');
        if(hex.Length != 6)
            throw new ArgumentException("Hex string must be 6 characters long.", nameof(hex));
        var r = byte.Parse(hex[..2], NumberStyles.HexNumber);
        var g = byte.Parse(hex[2..4], NumberStyles.HexNumber);
        var b = byte.Parse(hex[4..6], NumberStyles.HexNumber);
        return FromRgb(r, g, b, name);
    }

    /// <summary>
    /// Converts an AcoColor in RGB color space to a hexadecimal color string (e.g. #RRGGBB)
    /// </summary>
    /// <param name="color">The AcoColor to convert</param>
    /// <returns>A hexadecimal color string representing the specified AcoColor</returns>
    public static string ToHex(AcoColor color)
    {
        if(color.ColorSpace != ColorSpaceRgb)
            throw new ArgumentException("Color must be in RGB color space to convert to hex.", nameof(color));
        var r = (byte)(color.Components[0] / 257);
        var g = (byte)(color.Components[1] / 257);
        var b = (byte)(color.Components[2] / 257);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Creates an AcoColor from Cmyk values (0..100%)
    /// </summary>
    /// <param name="c">Cyan component (0..100%)</param>
    /// <param name="m">Magenta component (0..100%)</param>
    /// <param name="y">Yellow component (0..100%)</param>
    /// <param name="k">Black component (0..100%)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified CMYK color</returns>
    public static AcoColor FromCmyk(double c, double m, double y, double k, string name = "")
    {
        return new()
        {
            ColorSpace = ColorSpaceCmyk,
            Components =
            [
                (ushort)((1 - c / 100.0) * 65535),
                (ushort)((1 - m / 100.0) * 65535),
                (ushort)((1 - y / 100.0) * 65535),
                (ushort)((1 - k / 100.0) * 65535)
            ],
            Name = name
        };
    }

    /// <summary>
    /// Creates an AcoColor from Lab values.
    /// </summary>
    /// <param name="L">Lightness component (0..100%)</param>
    /// <param name="a">Green-Red component (-128..127)</param>
    /// <param name="b">Blue-Yellow component (-128..127)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified Lab color</returns>
    public static AcoColor FromLab(double L, double a, double b, string name = "")
    {
        return new()
        {
            ColorSpace = ColorSpaceLab,
            Components =
            [
                (ushort)(L / 100.0 * 65535),
                (ushort)((a + 128) / 255.0 * 65535),
                (ushort)((b + 128) / 255.0 * 65535),
                0
            ],
            Name = name
        };
    }

    /// <summary>
    /// Creates an AcoColor from a Gray value (0..100%)
    /// </summary>
    /// <param name="gray">Gray component (0..100%)</param>
    /// <param name="name">Optional name for the color</param>
    /// <returns>An AcoColor representing the specified Gray color</returns>
    public static AcoColor FromGray(double gray, string name = "")
    {
        return new()
        {
            ColorSpace = ColorSpaceGray,
            Components =
            [
                (ushort)(gray / 100.0 * 65535),
                0,0,0
            ],
            Name = name
        };
    }
}