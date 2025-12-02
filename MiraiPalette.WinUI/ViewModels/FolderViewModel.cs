using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.Shared.Entities;

namespace MiraiPalette.WinUI.ViewModels;

public partial class FolderViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    public static FolderViewModel DefaultFolder { get; } = new()
    {
        Id = 0,
        Name = "默认"
    };


    public FolderViewModel()
    {

    }

    public FolderViewModel(MiraiFolder folder)
    {
        Id = folder.Id;
        Name = folder.Name;
    }
}
