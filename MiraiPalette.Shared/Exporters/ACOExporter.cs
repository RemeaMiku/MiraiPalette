using MiraiPalette.Shared.Entities;

namespace MiraiPalette.Shared.Exporters;

[Flags]
public enum ACOFormatVersion
{
    Version1 = 1 << 0,
    Version2 = 1 << 1,
    Both = Version1 | Version2
}

public static class ACOExporter
{
    public static void ExportACO(string filePath, IEnumerable<MiraiColor> colors, ACOFormatVersion version)
    {
        using var stream = File.Open(filePath, FileMode.Create);
        using var writer = new BinaryWriter(stream);
        ExportACO(writer, colors, version);
    }

    public static void ExportACO(BinaryWriter writer, IEnumerable<MiraiColor> colors, ACOFormatVersion version)
    {
        if((version & ACOFormatVersion.Version1) != 0)
        {
            WriteVersion1(writer, colors);
        }

        if((version & ACOFormatVersion.Version2) != 0)
        {
            WriteVersion2(writer, colors);
        }
    }

    private static void WriteVersion1(BinaryWriter writer, IEnumerable<MiraiColor> colors)
    {
        writer.Write((ushort)1); // Version 1 header
        writer.Write((ushort)colors.Count());

        foreach(var color in colors)
        {
            WriteRGBColorEntry(writer, color);
        }
    }

    private static void WriteVersion2(BinaryWriter writer, IEnumerable<MiraiColor> colors)
    {
        writer.Write((ushort)2); // Version 2 header
        writer.Write((ushort)colors.Count());

        foreach(var color in colors)
        {
            WriteRGBColorEntry(writer, color);
            WriteColorName(writer, color.Name);
        }
    }

    private static void WriteRGBColorEntry(BinaryWriter writer, MiraiColor color)
    {
        writer.Write((ushort)0); // RGB color space

        var (r, g, b) = HexToRGB(color.Hex);
        writer.Write((ushort)(r * 256));
        writer.Write((ushort)(g * 256));
        writer.Write((ushort)(b * 256));
        writer.Write((ushort)0); // Reserved
    }

    private static void WriteColorName(BinaryWriter writer, string name)
    {
        var nameChars = name.ToCharArray();
        writer.Write((uint)nameChars.Length);
        foreach(char c in nameChars)
        {
            writer.Write((ushort)c);
        }
    }

    private static (byte r, byte g, byte b) HexToRGB(string hex)
    {
        if(hex.StartsWith('#'))
            hex = hex[1..];
        if(hex.Length != 6)
            throw new ArgumentException($"Invalid hex color: {hex}");

        byte r = Convert.ToByte(hex[..2], 16);
        byte g = Convert.ToByte(hex[2..4], 16);
        byte b = Convert.ToByte(hex[4..6], 16);
        return (r, g, b);
    }
}
