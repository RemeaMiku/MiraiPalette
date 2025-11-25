namespace MiraiPalette.Shared.Formats;

public class InvalidPaletteFileException: Exception
{
    public InvalidPaletteFileException()
    {
    }
    public InvalidPaletteFileException(string? message) : base(message)
    {
    }
    public InvalidPaletteFileException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
