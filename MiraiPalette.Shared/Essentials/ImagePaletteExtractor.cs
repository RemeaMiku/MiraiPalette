namespace MiraiPalette.Shared.Essentials;

public class ImagePaletteExtractor
{
    private class Cluster
    {
        public (double L, double A, double B) Center;
        public List<(double L, double A, double B)> Pixels = [];
    }

    public int MaxPixelCount { get; init; } = 1024 * 1024;
    public int MaxKMeansIterations { get; init; } = 25;

    /// <summary>
    /// 提取代表性颜色调色板。
    /// </summary>
    public IEnumerable<(byte R, byte G, byte B, float Percentage)>
        Extract(IEnumerable<(byte R, byte G, byte B)> pixels, int colorCount)
    {
        var source = pixels as (byte R, byte G, byte B)[] ?? [.. pixels];
        if(source.Length == 0)
            yield break;

        // 采样控制
        int step = Math.Max(1, source.Length / MaxPixelCount);
        var sampled = TakeEvery(source, step)
            .Select(p => RgbToLabWeighted(p.R, p.G, p.B))
            .ToArray();

        var clusters = KMeansPlusPlus(sampled, colorCount, MaxKMeansIterations);
        var total = (float)sampled.Length;

        foreach(var c in clusters.OrderByDescending(c => c.Pixels.Count))
        {
            if(c.Pixels.Count == 0)
                continue;
            var (L, A, B) = Average(c.Pixels);
            var (r, g, b) = LabToRgb(L, A, B);
            yield return ((byte)r, (byte)g, (byte)b, c.Pixels.Count * 100f / total);
        }
    }

    public static IEnumerable<T> TakeEvery<T>(IEnumerable<T> source, int step)
    {
        using var e = source.GetEnumerator();
        while(e.MoveNext())
        {
            yield return e.Current;
            for(int i = 1; i < step && e.MoveNext(); i++)
            { }
        }
    }

    // -------------------------------
    // 改良版 KMeans++ 聚类
    // -------------------------------
    private static List<Cluster> KMeansPlusPlus((double L, double A, double B)[] pixels, int k, int maxIter)
    {
        var rnd = new Random();
        var centers = new List<(double L, double A, double B)>
        {
            pixels[rnd.Next(pixels.Length)]
        };

        while(centers.Count < k)
        {
            var distances = pixels.Select(p => centers.Min(c => DistSq(p, c))).ToArray();
            double total = distances.Sum();
            double r = rnd.NextDouble() * total;
            double cum = 0;
            for(int i = 0; i < pixels.Length; i++)
            {
                cum += distances[i];
                if(cum >= r)
                {
                    centers.Add(pixels[i]);
                    break;
                }
            }
        }

        List<Cluster> clusters = [];
        for(int iter = 0; iter < maxIter; iter++)
        {
            clusters = centers.Select(c => new Cluster { Center = c }).ToList();

            foreach(var p in pixels)
            {
                int best = 0;
                double min = double.MaxValue;
                for(int i = 0; i < centers.Count; i++)
                {
                    double d = DistSq(p, centers[i]);
                    if(d < min)
                    {
                        min = d;
                        best = i;
                    }
                }
                clusters[best].Pixels.Add(p);
            }

            for(int i = 0; i < clusters.Count; i++)
            {
                if(clusters[i].Pixels.Count == 0)
                    continue;
                centers[i] = Average(clusters[i].Pixels);
            }
        }

        return clusters;
    }

    private static double DistSq((double L, double A, double B) p1, (double L, double A, double B) p2)
    {
        // ΔE近似公式（Lab空间距离）
        double dL = p1.L - p2.L;
        double dA = p1.A - p2.A;
        double dB = p1.B - p2.B;
        return 0.5 * dL * dL + dA * dA + dB * dB;
    }

    private static (double L, double A, double B) Average(IEnumerable<(double L, double A, double B)> pixels)
    {
        double l = 0, a = 0, b = 0;
        int n = 0;
        foreach(var (L, A, B) in pixels)
        { l += L; a += A; b += B; n++; }
        return (l / n, a / n, b / n);
    }

    // -------------------------------
    // 感知加权 RGB → Lab
    // -------------------------------
    private static (double L, double A, double B) RgbToLabWeighted(byte r, byte g, byte b)
    {
        double rf = r / 255.0, gf = g / 255.0, bf = b / 255.0;
        rf = rf <= 0.04045 ? rf / 12.92 : Math.Pow((rf + 0.055) / 1.055, 2.4);
        gf = gf <= 0.04045 ? gf / 12.92 : Math.Pow((gf + 0.055) / 1.055, 2.4);
        bf = bf <= 0.04045 ? bf / 12.92 : Math.Pow((bf + 0.055) / 1.055, 2.4);

        double x = rf * 0.4124 + gf * 0.3576 + bf * 0.1805;
        double y = rf * 0.2126 + gf * 0.7152 + bf * 0.0722;
        double z = rf * 0.0193 + gf * 0.1192 + bf * 0.9505;
        x /= 0.95047;
        z /= 1.08883;

        double fx = PivotLab(x), fy = PivotLab(y), fz = PivotLab(z);
        double L = 116 * fy - 16, A = 500 * (fx - fy), B = 200 * (fy - fz);

        // 感知加权（高饱和度像素影响更大）
        double sat = Math.Sqrt(A * A + B * B) / (L + 1);
        return (L * (1 + 0.3 * sat), A, B);
    }

    private static double PivotLab(double n)
        => n > 0.008856 ? Math.Pow(n, 1.0 / 3.0) : (7.787 * n) + (16.0 / 116.0);

    // -------------------------------
    // Lab → RGB
    // -------------------------------
    private static (byte R, byte G, byte B) LabToRgb(double L, double A, double B)
    {
        double y = (L + 16) / 116.0;
        double x = A / 500.0 + y;
        double z = y - B / 200.0;

        double X = 0.95047 * UnpivotLab(x);
        double Y = UnpivotLab(y);
        double Z = 1.08883 * UnpivotLab(z);

        double r = 3.2406 * X - 1.5372 * Y - 0.4986 * Z;
        double g = -0.9689 * X + 1.8758 * Y + 0.0415 * Z;
        double b = 0.0557 * X - 0.2040 * Y + 1.0570 * Z;

        static byte Clamp(double v)
            => (byte)Math.Clamp((v <= 0.0031308 ? 12.92 * v : 1.055 * Math.Pow(v, 1 / 2.4) - 0.055) * 255.0, 0, 255);

        return (Clamp(r), Clamp(g), Clamp(b));
    }

    private static double UnpivotLab(double n)
        => n * n * n > 0.008856 ? n * n * n : (n - 16.0 / 116.0) / 7.787;
}
