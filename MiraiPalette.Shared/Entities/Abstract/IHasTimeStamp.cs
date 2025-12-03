namespace MiraiPalette.Shared.Entities.Abstract;

public interface IHasTimeStamp
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
