using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.ViewModels;

public partial class FolderViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    public bool IsVirtual => IsVirtualFolder(Id);

    public static FolderViewModel AllPalettes { get; } = new() { Id = 0, Name = Resources.AllPalettes };

    public static FolderViewModel Unassigned { get; } = new() { Id = -1, Name = "Unassigned" };

    public static FolderViewModel Favorites { get; } = new() { Id = -2, Name = "Favorite" };

    public static bool IsVirtualFolder(int folderId) => folderId <= 0;
}
