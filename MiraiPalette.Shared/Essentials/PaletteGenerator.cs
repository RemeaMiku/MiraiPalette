namespace MiraiPalette.Shared.Essentials;

public static class PaletteGenerator
{
    public static IEnumerable<string> GenerateRandomHexColor(int count)
    {
        var random = new Random();
        var colors = new HashSet<string>();
        while(colors.Count < count)
        {
            var color = string.Format("#{0:X6}", random.Next(0x1000000));
            colors.Add(color);
        }
        return colors;
    }

    public static string GenerateRandomHexColor()
    {
        var random = new Random();
        return string.Format("#{0:X6}", random.Next(0x1000000));
    }
}
