using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.Shared.Entities;

namespace MiraiPalette.WinUI.ViewModels;

public partial class FolderViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    public static FolderViewModel AllPalettes { get; } = new() { Id = -1, Name = "All Palettes" };

    public static FolderViewModel Unassigned { get; } = new() { Id = -2, Name = "Unassigned" };

    public static FolderViewModel Favorites { get; } = new() { Id = -3, Name = "Favorite" };

    public FolderViewModel()
    {

    }

    public FolderViewModel(MiraiFolder folder)
    {
        Id = folder.Id;
        Name = folder.Name;
    }
}
