using SkiaSharp;

namespace MiraiPalette.Maui.Essentials;
public class ImagePaletteExtractor
{
    public class ImagePaletteColor
    {
        public int Percentage { get; set; }
        public Color Color { get; set; } = Colors.White;
    }

    public static async Task<IEnumerable<ImagePaletteColor>> ExtractAsync(string imagePath, int colorCount = 4)
    {
        return string.IsNullOrWhiteSpace(imagePath)
            ? throw new ArgumentException("Image path cannot be null or empty.", nameof(imagePath))
            : await Task.Run(() => Extract(imagePath, colorCount));
    }

    private static IEnumerable<ImagePaletteColor> Extract(string imagePath, int colorCount)
    {
        using var sampledBitmap = LoadAndSampleBitmap(imagePath, 128);
        var pixels = ExtractPixels(sampledBitmap);
        var clusters = ClusterColors(pixels, colorCount, 10);
        return ToPaletteColors(clusters, pixels.Count);
    }

    private static SKBitmap LoadAndSampleBitmap(string imagePath, int maxEdge)
    {
        using var stream = File.OpenRead(imagePath);
        using var originBitmap = SKBitmap.Decode(stream) ?? throw new InvalidOperationException("Failed to decode image.");
        int width = originBitmap.Width, height = originBitmap.Height;
        float scale = Math.Min(1f, (float)maxEdge / Math.Max(width, height));
        int targetW = (int)(width * scale);
        int targetH = (int)(height * scale);
        return (scale < 1f)
            ? originBitmap.Resize(new SKImageInfo(targetW, targetH), new SKSamplingOptions(SKFilterMode.Nearest))
            : originBitmap.Copy();
    }

    private static List<(byte R, byte G, byte B)> ExtractPixels(SKBitmap bitmap)
    {
        var pixels = new List<(byte R, byte G, byte B)>(bitmap.Width * bitmap.Height);
        for(int y = 0; y < bitmap.Height; y++)
        {
            for(int x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                pixels.Add((color.Red, color.Green, color.Blue));
            }
        }
        return pixels;
    }

    private static List<Cluster> ClusterColors(List<(byte R, byte G, byte B)> pixels, int k, int maxIterations)
    {
        return KMeans(pixels, k, maxIterations);
    }

    private static IEnumerable<ImagePaletteColor> ToPaletteColors(List<Cluster> clusters, int total)
    {
        foreach(var cluster in clusters.OrderByDescending(c => c.Pixels.Count))
        {
            if(cluster.Pixels.Count == 0)
                continue;
            int r = (int)cluster.Pixels.Average(p => p.R);
            int g = (int)cluster.Pixels.Average(p => p.G);
            int b = (int)cluster.Pixels.Average(p => p.B);
            double percent = cluster.Pixels.Count * 100.0 / total;
            yield return new ImagePaletteColor
            {
                Percentage = (int)Math.Round(percent),
                Color = Color.FromRgb(r, g, b)
            };
        }
    }

    private class Cluster
    {
        public (double R, double G, double B) Center;
        public List<(byte R, byte G, byte B)> Pixels = [];
    }

    private static List<Cluster> KMeans(List<(byte R, byte G, byte B)> pixels, int k, int maxIterations)
    {
        var rnd = new Random();
        var centers = pixels.OrderBy(_ => rnd.Next()).Take(k)
            .Select(p => (R: (double)p.R, G: (double)p.G, B: (double)p.B)).ToList();
        List<Cluster> clusters = [];
        for(int iter = 0; iter < maxIterations; iter++)
        {
            clusters = [.. centers.Select(c => new Cluster { Center = c })];
            foreach(var p in pixels)
            {
                int idx = 0;
                double minDist = double.MaxValue;
                for(int i = 0; i < centers.Count; i++)
                {
                    double dist = Math.Pow(p.R - centers[i].R, 2) + Math.Pow(p.G - centers[i].G, 2) + Math.Pow(p.B - centers[i].B, 2);
                    if(dist < minDist)
                    {
                        minDist = dist;
                        idx = i;
                    }
                }
                clusters[idx].Pixels.Add(p);
            }
            for(int i = 0; i < clusters.Count; i++)
            {
                if(clusters[i].Pixels.Count == 0)
                    continue;
                centers[i] = (
                    R: clusters[i].Pixels.Average(p => p.R),
                    G: clusters[i].Pixels.Average(p => p.G),
                    B: clusters[i].Pixels.Average(p => p.B)
                );
            }
        }
        return clusters;
    }
}
