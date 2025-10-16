using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI;
using Windows.UI;

namespace MiraiPalette.WinUI.Essentials;

public class ImagePixels(double scale, double width, double height, Color[] colors)
{
    public double Scale { get; } = scale;
    public double Width { get; } = width;
    public double Height { get; } = height;
    public Color[] Pixels { get; } = colors;

    public Color this[int x, int y]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(y, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, (int)Width);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, (int)Height);
            return Pixels[(int)(y * Width + x)];
        }
    }

    public Color FromOriginalCoord(int xOnOriginal, int yOnOriginal)
    {
        var sx = xOnOriginal * Scale;
        var sy = yOnOriginal * Scale;
        if(sx < 0 || sy < 0 || sx >= Width || sy >= Height)
            return Colors.Transparent;
        return Pixels[(int)(sy * Width + sx)];
    }
}

