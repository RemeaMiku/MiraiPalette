using System.Collections.Concurrent;
using System.Numerics;

namespace MiraiPalette.Shared.Essentials;

public class ImagePaletteExtractor
{
    private class Cluster
    {
        public (float L, float A, float B) Center;
        public ConcurrentBag<(float L, float A, float B)> Pixels = new();
    }

    public int MaxPixelCount { get; init; } = 1024 * 1024;
    public int MaxKMeansIterations { get; init; } = 25;

    /// <summary>
    /// 提取代表性颜色调色板。
    /// </summary>
    public IEnumerable<(byte R, byte G, byte B, float Percentage)>
        Extract(IEnumerable<(byte R, byte G, byte B)> pixels, int colorCount)
    {
        var source = pixels as (byte R, byte G, byte B)[] ?? pixels.ToArray();
        if(source.Length == 0)
            yield break;

        // ↓ 采样控制（防止过多像素）
        int step = Math.Max(1, source.Length / MaxPixelCount);
        var sampled = TakeEvery(source, step)
            .Select(p => RgbToLabWeighted(p.R, p.G, p.B))
            .ToArray();

        // ↓ MiniBatch + 并行化的 KMeans++
        var clusters = KMeansPlusPlus(sampled, colorCount, MaxKMeansIterations);
        var total = (float)sampled.Length;
        var totalAssigned = clusters.Sum(c => c.Pixels.Count);
        foreach(var c in clusters.OrderByDescending(c => c.Pixels.Count))
        {
            if(c.Pixels.IsEmpty)
                continue;
            var (L, A, B) = Average(c.Pixels);
            var (r, g, b) = LabToRgb(L, A, B);
            yield return ((byte)r, (byte)g, (byte)b, c.Pixels.Count * 100f / totalAssigned);
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

    // --------------------------------------------------------------------
    // MiniBatch + 并行版 KMeans++
    // --------------------------------------------------------------------
    private static List<Cluster> KMeansPlusPlus((float L, float A, float B)[] pixels, int k, int maxIter)
    {
        var rnd = new Random();
        var centers = new List<(float L, float A, float B)>(k)
        {
            pixels[rnd.Next(pixels.Length)]
        };

        // 初始化剩余中心点
        while(centers.Count < k)
        {
            var distances = new float[pixels.Length];
            for(int i = 0; i < pixels.Length; i++)
            {
                var p = pixels[i];
                float min = float.MaxValue;
                foreach(var c in centers)
                {
                    var d = DistSq(p, c);
                    if(d < min)
                        min = d;
                }
                distances[i] = min;
            }

            double total = 0;
            for(int i = 0; i < distances.Length; i++)
                total += distances[i];
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

        var clusters = centers.Select(c => new Cluster { Center = c }).ToList();
        const int batchSize = 8192;

        for(int iter = 0; iter < maxIter; iter++)
        {
            // 清空旧像素分配
            foreach(var c in clusters)
                c.Pixels = new();

            // 采样一个 batch（MiniBatch KMeans）
            var batch = new (float L, float A, float B)[batchSize];
            for(int i = 0; i < batchSize; i++)
                batch[i] = pixels[rnd.Next(pixels.Length)];

            // 并行计算归属
            Parallel.For(0, batch.Length, i =>
            {
                var p = batch[i];
                int best = 0;
                float min = float.MaxValue;
                for(int j = 0; j < centers.Count; j++)
                {
                    var c = centers[j];
                    float d = DistSq(p, c);
                    if(d < min)
                    {
                        min = d;
                        best = j;
                    }
                }
                clusters[best].Pixels.Add(p);
            });

            // 更新中心
            for(int i = 0; i < clusters.Count; i++)
            {
                if(clusters[i].Pixels.IsEmpty)
                    continue;
                centers[i] = Average(clusters[i].Pixels);
            }
        }

        return clusters;
    }

    private static float DistSq((float L, float A, float B) p1, (float L, float A, float B) p2)
    {
        // 向量化距离计算
        var v1 = new Vector3(p1.L, p1.A, p1.B);
        var v2 = new Vector3(p2.L, p2.A, p2.B);
        var d = v1 - v2;
        return Vector3.Dot(d, d);
    }

    private static (float L, float A, float B) Average(IEnumerable<(float L, float A, float B)> pixels)
    {
        float l = 0, a = 0, b = 0;
        int n = 0;
        foreach(var (L, A, B) in pixels)
        {
            l += L;
            a += A;
            b += B;
            n++;
        }
        return (l / n, a / n, b / n);
    }

    // -------------------------------
    // 感知加权 RGB → Lab
    // -------------------------------
    private static (float L, float A, float B) RgbToLabWeighted(byte r, byte g, byte b)
    {
        float rf = r / 255f, gf = g / 255f, bf = b / 255f;
        rf = rf <= 0.04045f ? rf / 12.92f : MathF.Pow((rf + 0.055f) / 1.055f, 2.4f);
        gf = gf <= 0.04045f ? gf / 12.92f : MathF.Pow((gf + 0.055f) / 1.055f, 2.4f);
        bf = bf <= 0.04045f ? bf / 12.92f : MathF.Pow((bf + 0.055f) / 1.055f, 2.4f);

        float x = rf * 0.4124f + gf * 0.3576f + bf * 0.1805f;
        float y = rf * 0.2126f + gf * 0.7152f + bf * 0.0722f;
        float z = rf * 0.0193f + gf * 0.1192f + bf * 0.9505f;
        x /= 0.95047f;
        z /= 1.08883f;

        float fx = PivotLab(x), fy = PivotLab(y), fz = PivotLab(z);
        float L = 116f * fy - 16f, A = 500f * (fx - fy), B = 200f * (fy - fz);
        float sat = MathF.Sqrt(A * A + B * B) / (L + 1f);
        return (L * (1f + 0.3f * sat), A, B);
    }

    private static float PivotLab(float n)
        => n > 0.008856f ? MathF.Pow(n, 1f / 3f) : (7.787f * n) + (16f / 116f);

    // -------------------------------
    // Lab → RGB
    // -------------------------------
    private static (byte R, byte G, byte B) LabToRgb(float L, float A, float B)
    {
        float y = (L + 16f) / 116f;
        float x = A / 500f + y;
        float z = y - B / 200f;

        float X = 0.95047f * UnpivotLab(x);
        float Y = UnpivotLab(y);
        float Z = 1.08883f * UnpivotLab(z);

        float r = 3.2406f * X - 1.5372f * Y - 0.4986f * Z;
        float g = -0.9689f * X + 1.8758f * Y + 0.0415f * Z;
        float b = 0.0557f * X - 0.2040f * Y + 1.0570f * Z;

        static byte Clamp(float v)
            => (byte)Math.Clamp((v <= 0.0031308f ? 12.92f * v : 1.055f * MathF.Pow(v, 1f / 2.4f) - 0.055f) * 255f, 0, 255);

        return (Clamp(r), Clamp(g), Clamp(b));
    }

    private static float UnpivotLab(float n)
        => n * n * n > 0.008856f ? n * n * n : (n - 16f / 116f) / 7.787f;
}
