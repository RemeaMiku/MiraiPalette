using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IMessenger messenger)
    : ObservableRecipient(messenger)
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    public ObservableCollection<FolderViewModel> Folders { get; } = [];

    public List<FolderViewModel> SpecialFolders { get; } =
        [
            FolderViewModel.AllPalettes,
        ];

    [ObservableProperty]
    public partial object? SelectedMenuItem { get; set; }

    [ObservableProperty]
    public partial FolderViewModel? SelectedSpecialFolder { get; set; }

    [RelayCommand]
    async Task Load()
    {
        Folders.Clear();
        foreach(var folder in await miraiPaletteStorageService.GetAllFoldersAsync())
            Folders.Add(folder);
        SelectedSpecialFolder = FolderViewModel.AllPalettes;
        IsActive = true;
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
        if(value is not FolderViewModel folder)
        {
            SelectedSpecialFolder = default;
            //Current.NavigateTo(NavigationTarget.Settings);
        }
        else
        {
            if(!SpecialFolders.Contains(folder))
                SelectedSpecialFolder = default;
            //Current.NavigateTo(NavigationTarget.Main, folder);
        }
    }

    partial void OnSelectedSpecialFolderChanged(FolderViewModel? value)
    {
        if(value is null)
            return;
        SelectedMenuItem = value;
    }

    //public void Receive(NavigationMessage message)
    //{
    //    if(message.Target == NavigationTarget.Main)
    //    {
    //        if(message.Parameter is not FolderViewModel folder)
    //            throw new ArgumentException("Parameter must be of type FolderViewModel", nameof(message));
    //        SelectedMenuItem = SpecialFolders.Contains(folder) || Folders.Contains(folder) ?
    //            folder : throw new ArgumentException("FolderViewModel not found", nameof(message));
    //    }
    //}
}
