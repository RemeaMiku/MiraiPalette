using System.Text;

namespace MiraiPalette.Shared.Formats.Aco;

/// <summary>
/// Represents an Adobe Color Swatch (ACO) file, providing methods to load and save color swatch data in ACO format.
/// </summary>
/// <remarks>An ACO file contains a palette of colors used by Adobe applications such as Photoshop. This class
/// supports both version 1 and version 2 of the ACO file format. Use the static Load methods to read an ACO file from a
/// stream or file path, and the Save methods to write the current color palette to a stream or file. The Colors
/// property contains the list of color entries in the palette. The Version property indicates the version of the ACO
/// file that was loaded or saved. Thread safety is not guaranteed; synchronize access if using instances across
/// multiple threads.</remarks>
public class AcoFile
{
    public const int DefaultVersion = 2;

    /// <summary>
    /// Gets or sets the version of the ACO file format (1 or 2).
    /// </summary>
    public int Version { get; set; } = DefaultVersion;

    /// <summary>
    /// Gets or sets the collection of colors contained in the palette.
    /// </summary>
    public List<AcoColor> Colors { get; set; } = [];

    /// <summary>
    /// Loads an ACO file from the specified stream.
    /// </summary>
    /// <param name="stream"> The stream to read the ACO file from. Must be readable and seekable.</param>
    /// <returns> An AcoFile instance representing the loaded color palette.</returns>
    /// <exception cref="InvalidPaletteFileException"> Thrown if the file format is invalid or unsupported.</exception>
    public static AcoFile Load(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.BigEndianUnicode, leaveOpen: true);
        var acoFile = new AcoFile();
        // ---- 第一段 (Version 1 区) ----
        var ver1 = ReadUInt16BE(reader);
        var countVer1 = ReadUInt16BE(reader);
        if(ver1 != 1)
            throw new InvalidPaletteFileException("ACO: first block must be version 1");
        for(int i = 0; i < countVer1; i++)
        {
            var color = ReadColorVer1(reader);
            acoFile.Colors.Add(color);
        }

        // 如果文件到这里结束，说明是 v1 文件
        if(stream.Position >= stream.Length)
        {
            acoFile.Version = 1;
            return acoFile;
        }

        // ---- 第二段 (Version 2 区) ----
        var ver2 = ReadUInt16BE(reader);
        var count2 = ReadUInt16BE(reader);
        if(ver2 != 2 || count2 != countVer1)
            throw new InvalidPaletteFileException("ACO: invalid version 2 block");
        for(int i = 0; i < count2; i++)
        {
            var color = ReadColorVer2(reader);
            acoFile.Colors[i] = color;
        }
        acoFile.Version = 2;
        return acoFile;
    }

    private static AcoColor ReadColorVer1(BinaryReader br)
    {
        var color = new AcoColor
        {
            ColorSpace = ReadUInt16BE(br)
        };
        for(int j = 0; j < 4; j++)
            color.Components[j] = ReadUInt16BE(br);
        return color;
    }

    private static AcoColor ReadColorVer2(BinaryReader br)
    {
        var color = new AcoColor
        {
            ColorSpace = ReadUInt16BE(br)
        };
        for(int j = 0; j < 4; j++)
            color.Components[j] = ReadUInt16BE(br);
        uint nameLength = ReadUInt32BE(br);
        var chars = new char[nameLength];
        for(int j = 0; j < nameLength; j++)
            chars[j] = (char)ReadUInt16BE(br);
        color.Name = new string(chars).TrimEnd('\0');
        return color;
    }

    /// <summary>
    /// Loads an AcoFile from the specified file path.
    /// </summary>
    /// <param name="path">The path to the file to load. The file must exist and be accessible for reading.</param>
    /// <returns>An AcoFile instance containing the data loaded from the specified file.</returns>
    public static AcoFile Load(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return Load(stream);
    }

    /// <summary>
    /// Saves the current AcoFile to the specified stream in the given version (1 or 2).
    /// </summary>
    /// <param name="stream">The stream to write the AcoFile data to. Must be writable and seekable.</param>
    /// <param name="version">The version of the AcoFile format to use (1 or 2).</param>
    /// <exception cref="NotSupportedException">Thrown if the specified version is not supported.</exception>
    public void Save(Stream stream, int version = DefaultVersion)
    {
        using var writer = new BinaryWriter(stream, Encoding.BigEndianUnicode, leaveOpen: true);

        if(version is < 1 or > 2)
            throw new NotSupportedException("ACO version must be 1 or 2");

        // 写 Version 1 区
        WriteUInt16BE(writer, 1);
        WriteUInt16BE(writer, (ushort)Colors.Count);
        foreach(var color in Colors)
        {
            WriteUInt16BE(writer, color.ColorSpace);
            for(int j = 0; j < 4; j++)
                WriteUInt16BE(writer, color.Components[j]);
        }

        if(version == 2)
        {
            WriteUInt16BE(writer, 2);
            WriteUInt16BE(writer, (ushort)Colors.Count);
            foreach(var color in Colors)
            {
                WriteUInt16BE(writer, color.ColorSpace);
                for(int j = 0; j < 4; j++)
                    WriteUInt16BE(writer, color.Components[j]);
                var name = color.Name ?? string.Empty;
                var length = name.Length + 1;
                WriteUInt32BE(writer, (uint)length);
                foreach(char c in name)
                    WriteUInt16BE(writer, c);
                WriteUInt16BE(writer, 0); // null terminator
            }
        }
    }

    /// <summary>
    /// Saves the current AcoFile to the specified file path in the given version (1 or 2).
    /// </summary>
    /// <param name="path">The path to the file to save.</param>
    /// <param name="version">The version of the AcoFile format to use (1 or 2).</param>
    public void Save(string path, int version = 2)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Save(stream, version);
    }

    private static ushort ReadUInt16BE(BinaryReader br)
    {
        var data = br.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToUInt16(data, 0);
    }

    private static uint ReadUInt32BE(BinaryReader br)
    {
        var data = br.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToUInt32(data, 0);
    }

    private static void WriteUInt16BE(BinaryWriter bw, ushort value)
    {
        var data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        bw.Write(data);
    }

    private static void WriteUInt32BE(BinaryWriter bw, uint value)
    {
        var data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        bw.Write(data);
    }
}
