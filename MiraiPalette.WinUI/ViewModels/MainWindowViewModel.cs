using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    readonly IMiraiPaletteStorageService _miraiPaletteStorageService;

    public MainWindowViewModel(IMiraiPaletteStorageService miraiPaletteStorageService)
    {
        _miraiPaletteStorageService = miraiPaletteStorageService;
    }

    [ObservableProperty]
    public partial string Title { get; set; } = "Mirai Palette";

    public ObservableCollection<FolderViewModel> Folders { get; } = [];

    [ObservableProperty]
    public partial FolderViewModel SelectedFolder { get; set; } = FolderViewModel.DefaultFolder;

    [RelayCommand]
    async Task Load()
    {
        if(Folders.Count != 0)
            Folders.Clear();
        foreach(var folder in await _miraiPaletteStorageService.GetAllFoldersAsync())
            Folders.Add(folder);
    }

    async partial void OnSelectedFolderChanged(FolderViewModel oldValue, FolderViewModel newValue)
    {
        await Current.ShowConfirmDialogAsync("DEV", newValue.Name);
    }

}
