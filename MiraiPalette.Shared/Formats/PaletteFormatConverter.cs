using MiraiPalette.Shared.Entities;
using MiraiPalette.Shared.Formats.Aco;

namespace MiraiPalette.Shared.Formats;

public static class PaletteFormatConverter
{
    extension(MiraiColor color)
    {
        public AcoColor ToAcoColor() =>
        AcoColor.FromHex(color.Hex, color.Name);

        public static MiraiColor FromAcoColor(AcoColor acoColor) =>
            new()
            {
                Name = acoColor.Name,
                Hex = AcoColor.ToHex(acoColor)
            };
    }

    extension(Palette palette)
    {
        public AcoFile CreateAcoFile()
        {
            var acoFile = new AcoFile
            {
                Colors = [.. palette.Colors.Select(ToAcoColor)]
            };
            return acoFile;
        }

        public static Palette FromAcoFile(AcoFile acoFile) =>
            new()
            {
                Colors = [.. acoFile.Colors.Select(FromAcoColor)]
            };
    }



}
