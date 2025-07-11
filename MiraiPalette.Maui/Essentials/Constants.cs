using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.Essentials;

public static class Constants
{
    public const string DefaultColorAsHex = "#FFF";

    public static string DefaultColorName { get; } = StringResource.NewColor;

    public static string DefaultPaletteName { get; } = StringResource.NewPalette;

    public const int DefaultColorCount = 4;
}