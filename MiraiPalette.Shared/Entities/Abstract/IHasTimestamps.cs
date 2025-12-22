namespace MiraiPalette.Shared.Entities.Abstract;

public interface IHasTimestamps
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
