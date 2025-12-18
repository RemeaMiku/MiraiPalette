namespace MiraiPalette.WinUI.Messaging;

public class FolderDeletedMessage(int folderId)
{
    public int FolderId { get; } = folderId;
}
