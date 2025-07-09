namespace MiraiPalette.Maui.Essentials;
public class ImagePaletteExtractor
{
    public class ImagePaletteColor
    {
        public int Percentage { get; set; }
        public Color Color { get; set; } = Colors.White;
    }

    public static async Task<IEnumerable<ImagePaletteColor>> ExtractAsync(string imagePath)
    {
        if(string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Image path cannot be null or empty.", nameof(imagePath));
        var image = ImageSource.FromFile(imagePath) ?? throw new InvalidOperationException("Failed to load image from the specified path.");
        // Use a library or algorithm to extract colors from the image.
        // This is a placeholder for actual color extraction logic.
        var colors = new List<ImagePaletteColor>
        {
            new() { Percentage = 67, Color = Colors.Red },
            new() { Percentage = 33, Color = Colors.Green }
        };
        return colors;
    }
}
