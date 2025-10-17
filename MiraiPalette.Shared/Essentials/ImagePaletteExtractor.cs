namespace MiraiPalette.Shared.Essentials;

/// <summary>
/// Provides functionality to extract a representative color palette from an image using k-means clustering.
/// </summary>
/// <remarks>Use this class to analyze a collection of pixel colors and determine the most prominent colors
/// present, along with their relative proportions. The palette extraction is performed using the k-means algorithm, and
/// the number of clusters (colors) can be specified. The extraction process is suitable for applications such as image
/// analysis, color theme generation, or automatic palette selection.</remarks>
public class ImagePaletteExtractor
{
    private class Cluster
    {
        public (double R, double G, double B) Center { get; init; }
        public List<(byte R, byte G, byte B)> Pixels { get; } = [];
    }

    public int MaxKMeansIterations { get; init; } = 25;

    public IEnumerable<(byte R, byte G, byte B, float Percentage)> Extract(IEnumerable<(byte R, byte G, byte B)> pixels, int colorCount)
    {
        var clusters = ClusterColors(pixels, colorCount, MaxKMeansIterations);
        return ToPaletteColors(clusters, pixels.Count());
    }

    private static IEnumerable<(byte R, byte G, byte B, float Percentage)> ToPaletteColors(IEnumerable<Cluster> clusters, int total)
    {
        foreach(var cluster in clusters.OrderByDescending(c => c.Pixels.Count))
        {
            if(cluster.Pixels.Count == 0)
                continue;
            var r = (int)cluster.Pixels.Average(p => p.R);
            var g = (int)cluster.Pixels.Average(p => p.G);
            var b = (int)cluster.Pixels.Average(p => p.B);
            var percent = cluster.Pixels.Count * 100f / total;
            yield return new()
            {
                Percentage = percent,
                R = (byte)r,
                G = (byte)g,
                B = (byte)b
            };
        }
    }

    private static List<Cluster> ClusterColors(IEnumerable<(byte R, byte G, byte B)> pixels, int k, int maxIterations)
    {
        var seed = DateTimeOffset.Now.GetHashCode();
        return KMeans(pixels, k, maxIterations, seed);
    }

    private static List<Cluster> KMeans(IEnumerable<(byte R, byte G, byte B)> pixels, int k, int maxIterations, int seed = 0)
    {
        var rnd = new Random(seed);
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
