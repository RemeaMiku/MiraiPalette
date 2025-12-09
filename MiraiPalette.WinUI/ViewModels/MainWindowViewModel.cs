using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel(IMiraiPaletteStorageService miraiPaletteStorageService) : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    public ObservableCollection<FolderViewModel> Folders { get; } = [];

    public List<FolderViewModel> SpecialFolders { get; } =
        [
            FolderViewModel.AllPalettes,
        ];

    public FolderViewModel? SelectedFolder
    {
        get
        {
            return SelectedSpecialFolder is not null
                ? SelectedSpecialFolder
                : SelectedMenuItem is not null and FolderViewModel folder ? folder : default;
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedFolder))]
    public partial object? SelectedMenuItem { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedFolder))]
    public partial FolderViewModel? SelectedSpecialFolder { get; set; }

    [RelayCommand]
    async Task Load()
    {
        Folders.Clear();
        foreach(var folder in await miraiPaletteStorageService.GetAllFoldersAsync())
            Folders.Add(folder);
        if(SelectedFolder is null && SelectedSpecialFolder is null)
            SelectedSpecialFolder = FolderViewModel.AllPalettes;
    }

    [RelayCommand]
    async Task AddFolder()
    {
        var folder = new FolderViewModel
        {
            Name = "New Folder",
        };
        await miraiPaletteStorageService.AddFolderAsync(folder);
        Folders.Add(folder);
        SelectedMenuItem = folder;
    }

    partial void OnSelectedMenuItemChanged(object? value)
    {
        if(value is null)
            return;
        SelectedSpecialFolder = default;
        if(SelectedFolder is not null)
            Current.NavigateTo(NavigationTarget.Main, SelectedFolder);
    }

    partial void OnSelectedSpecialFolderChanged(FolderViewModel? value)
    {
        if(value is null)
            return;
        Current.NavigateTo(NavigationTarget.Main, value);
        SelectedMenuItem = default;
    }
}
