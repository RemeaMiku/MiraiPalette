namespace MiraiPalette.Shared.Entities;

public class MiraiPalette
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }

    public int? FolderId { get; set; }
    public MiraiFolder? Folder { get; set; }

    public List<MiraiColor> Colors { get; set; } = [];
    public List<MiraiTag> Tags { get; set; } = [];
}