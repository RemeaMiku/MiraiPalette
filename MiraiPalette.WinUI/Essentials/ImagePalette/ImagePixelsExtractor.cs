using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;

namespace MiraiPalette.WinUI.Essentials.ImagePalette;

public class ImagePixelsExtractor
{
    public int MaxPixelsCount { get; init; } = int.MaxValue;

    public async Task<ImagePixels> ExtractImagePixelsAsync(string path)
    {
        var file = await StorageFile.GetFileFromPathAsync(path);
        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);

        var w = decoder.PixelWidth;
        var h = decoder.PixelHeight;

        var scale = Math.Min(1.0, Math.Sqrt((double)MaxPixelsCount / (w * h)));
        var scaledW = (uint)Math.Round(w * scale);
        var scaledH = (uint)Math.Round(h * scale);

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
            ExifOrientationMode.RespectExifOrientation,
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

        return new ImagePixels(scale, (int)scaledW, (int)scaledH, colors);
    }

}