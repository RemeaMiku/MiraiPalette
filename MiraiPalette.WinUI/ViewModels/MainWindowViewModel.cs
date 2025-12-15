using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IMessenger messenger)
    : ObservableRecipient(messenger),
    IRecipient<NavigationMessage>
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    public ObservableCollection<FolderViewModel> Folders { get; } = [];

    public List<FolderViewModel> SpecialFolders { get; } =
        [
            FolderViewModel.AllPalettes,
        ];

    //public FolderViewModel? SelectedFolder
    //{
    //    get
    //    {
    //        return SelectedSpecialFolder is not null
    //            ? SelectedSpecialFolder
    //            : SelectedMenuItem is not null and FolderViewModel folder ? folder : default;
    //    }
    //}

    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(SelectedFolder))]
    public partial object? SelectedMenuItem { get; set; }

    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(SelectedFolder))]
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
        //if((value is not FolderViewModel) || (value is FolderViewModel folder && !SpecialFolders.Contains(folder)))
        //    SelectedSpecialFolder = default;
        //Current.NavigateTo(NavigationTarget.Main, SelectedFolder);
        if(value is not FolderViewModel folder)
        {
            SelectedSpecialFolder = default;
            Current.NavigateTo(NavigationTarget.Settings);
        }
        else
        {
            if(!SpecialFolders.Contains(folder))
                SelectedSpecialFolder = default;
            Current.NavigateTo(NavigationTarget.Main, folder);
        }
    }

    partial void OnSelectedSpecialFolderChanged(FolderViewModel? value)
    {
        if(value is null)
            return;
        SelectedMenuItem = value;
    }

    public void Receive(NavigationMessage message)
    {
        if(message.Target == NavigationTarget.Main)
        {
            if(message.Parameter is not FolderViewModel folder)
                throw new ArgumentException("Parameter must be of type FolderViewModel", nameof(message));
            if(SpecialFolders.Contains(folder))
                SelectedSpecialFolder = folder;
            else if(Folders.Contains(folder))
                SelectedMenuItem = folder;
            else
                throw new ArgumentException("FolderViewModel not found", nameof(message));
        }
    }
}
