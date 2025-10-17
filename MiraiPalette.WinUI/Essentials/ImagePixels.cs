using System;
using System.Diagnostics;
using Microsoft.UI;
using Windows.UI;

namespace MiraiPalette.WinUI.Essentials;

public class ImagePixels(double scale, int width, int height, Color[] colors)
{
    public double Scale { get; } = scale;
    public int Width { get; } = width;
    public int Height { get; } = height;
    public Color[] PixelData { get; } = colors;

    public Color this[int x, int y]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(y, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);
            return PixelData[y * Width + x];
        }
    }

    public Color FromOriginalCoord(int xOnOriginal, int yOnOriginal)
    {
        var sx = xOnOriginal * Scale;
        var sy = yOnOriginal * Scale;
        Trace.WriteLine("");
        Trace.WriteLine($"ox:{xOnOriginal},oy:{yOnOriginal}");
        Trace.WriteLine($"sx:{sx:F0},sy:{sy:F0}");
        return sx < 0 || sy < 0 || sx >= Width || sy >= Height ? Colors.Transparent : PixelData[(int)Math.Round(sy * Width + sx)];
    }
}

