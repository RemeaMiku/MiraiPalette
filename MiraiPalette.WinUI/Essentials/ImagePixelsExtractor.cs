using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace MiraiPalette.WinUI.Essentials;

public class ImagePixelsExtractor
{
    public int MaxPixelsCount { get; init; } = 1024 * 1024;

    public async Task<ImagePixels> ExtractImagePixelsAsync(string path)
    {
        var file = await StorageFile.GetFileFromPathAsync(path);
        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);

        var w = decoder.PixelWidth;
        var h = decoder.PixelHeight;

        var scale = Math.Min(1.0, (double)MaxPixelsCount / Math.Max(w, h));
        var scaledW = (uint)(w * scale);
        var scaledH = (uint)(h * scale);

        var transform = new BitmapTransform()
        {
            ScaledWidth = scaledW,
            ScaledHeight = scaledH,
            InterpolationMode = BitmapInterpolationMode.Fant
        };

        var pixelData = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            transform,
            ExifOrientationMode.IgnoreExifOrientation,
            ColorManagementMode.DoNotColorManage);

        var bytes = pixelData.DetachPixelData();
        var colors = new Color[scaledW * scaledH];

        for(int i = 0; i < colors.Length; i++)
        {
            int idx = i * 4;
            colors[i] = Color.FromArgb(
                bytes[idx + 3],
                bytes[idx + 2],
                bytes[idx + 1],
                bytes[idx]);
        }

        return new ImagePixels(scale, scaledW, scaledH, colors);
    }

}