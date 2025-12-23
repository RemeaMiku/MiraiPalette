using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Messaging;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IMessenger messenger)
    : ObservableRecipient(messenger), IRecipient<FolderDeletedMessage>
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    public ObservableCollection<FolderViewModel> Folders { get; } = [];

    /// <summary>
    /// 必须在运行时加载，否则会导致本地化文本无法正确显示
    /// </summary>
    public ObservableCollection<FolderViewModel> SpecialFolders { get; } = [];

    [ObservableProperty]
    public partial object? SelectedMenuItem { get; set; }

    [ObservableProperty]
    public partial FolderViewModel? SelectedSpecialFolder { get; set; }

    [RelayCommand]
    async Task Load()
    {
        SpecialFolders.Clear();
        Folders.Clear();
        foreach(var folder in await miraiPaletteStorageService.GetAllFoldersAsync())
            Folders.Add(folder);
        SpecialFolders.Add(FolderViewModel.AllPalettes);
        SelectedSpecialFolder = FolderViewModel.AllPalettes;
        IsActive = true;
    }

    [RelayCommand]
    async Task AddFolder()
    {
        var folder = new FolderViewModel
        {
            Name = $"{Resources.DefaultFolderName} {Folders.Count + 1}",
        };
        await miraiPaletteStorageService.AddFolderAsync(folder);
        Folders.Add(folder);
        SelectedMenuItem = folder;
        NewFolderAdded?.Invoke(this, folder);
    }

    public EventHandler<FolderViewModel>? NewFolderAdded { get; set; }

    partial void OnSelectedMenuItemChanged(object? value)
    {
        if(value is null)
            return;
        if(value is not FolderViewModel folder)
        {
            SelectedSpecialFolder = default;
            Messenger.Send(new NavigationMessage(NavigationTarget.Settings));
        }
        else
        {
            if(!SpecialFolders.Contains(folder))
                SelectedSpecialFolder = default;
            Messenger.Send(new NavigationMessage(NavigationTarget.Main, folder));
        }
    }

    partial void OnSelectedSpecialFolderChanged(FolderViewModel? value)
    {
        if(value is null)
            return;
        SelectedMenuItem = value;
    }

    public bool FolderCanMoveUp(FolderViewModel folder)
        => Folders.IndexOf(folder) > 0;

    public bool FolderCanMoveDown(FolderViewModel folder)
        => Folders.IndexOf(folder) < Folders.Count - 1;

    void FolderMoveUp(FolderViewModel folder)
    {
        if(!FolderCanMoveUp(folder))
            throw new InvalidOperationException("Folder cannot be moved up");
        Folders.Move(Folders.IndexOf(folder), Folders.IndexOf(folder) - 1);
    }

    public void Receive(FolderDeletedMessage message)
    {
        var folder = Folders.FirstOrDefault(f => f.Id == message.FolderId) ??
            throw new InvalidOperationException("Folder not found");
        if(SelectedMenuItem == folder)
            SelectedSpecialFolder = FolderViewModel.AllPalettes;
        Folders.Remove(folder);
    }
}
