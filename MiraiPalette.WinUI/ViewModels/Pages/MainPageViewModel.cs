using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Messaging;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainPageViewModel : PageViewModelBase
{
    public MainPageViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IPaletteFileService paletteFileService, IMessenger messenger) : base(messenger)
    {
        _miraiPaletteStorageService = miraiPaletteStorageService;
        _paletteFileService = paletteFileService;
        SelectedPalettes.CollectionChanged += OnSelectedPalettesChanged;
        PaletteCommand = NavigateToPaletteCommand;
    }

    private void OnSelectedPalettesChanged(object? _, NotifyCollectionChangedEventArgs __)
    {
        OnPropertyChanged(nameof(HasSelectedPalettes));
        OnPropertyChanged(nameof(IsAllPalettesSelected));
    }

    [ObservableProperty]
    public partial FolderViewModel Folder { get; set; } = null!;

    partial void OnFolderChanged(FolderViewModel oldValue, FolderViewModel newValue)
    {
        oldValue?.PropertyChanged -= OnFolderPropertyChanged;
        newValue?.PropertyChanged += OnFolderPropertyChanged;
    }

    private async void OnFolderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(FolderViewModel.Name))
        {
            if(string.IsNullOrWhiteSpace(Folder.Name))
            {
                Folder.Name = (await _miraiPaletteStorageService.GetFolderAsync(Folder.Id))!.Name;
                return;
            }
            await _miraiPaletteStorageService.UpdateFolderAsync(Folder);
        }
    }


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPalettes))]
    public partial ObservableCollection<PaletteViewModel> Palettes { get; set; } = [];

    public bool HasPalettes => Palettes.Count > 0;

    partial void OnPalettesChanged(ObservableCollection<PaletteViewModel> oldValue, ObservableCollection<PaletteViewModel> newValue)
    {
        if(oldValue is not null)
            oldValue.CollectionChanged -= OnPalettesCollectionChanged;
        if(newValue is not null)
            newValue.CollectionChanged += OnPalettesCollectionChanged;
    }

    private void OnPalettesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasPalettes));
    }

    readonly IMiraiPaletteStorageService _miraiPaletteStorageService;

    readonly IPaletteFileService _paletteFileService;

    public ObservableCollection<PaletteViewModel> SelectedPalettes { get; } = [];

    [ObservableProperty]
    public partial PaletteViewModel CurrentPalette { get; set; } = null!;

    public bool HasSelectedPalettes => SelectedPalettes.Count > 0;

    public bool IsAllPalettesSelected => Palettes.Count > 0 && SelectedPalettes.Count == Palettes.Count;

    [ObservableProperty]
    public partial bool IsMultiSelectMode { get; set; } = false;

    [ObservableProperty]
    public partial ICommand PaletteCommand { get; set; }

    public override async void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        if(parameter is null && Folder is not null)
        {
            await Load(Folder);
            return;
        }
        ArgumentNullException.ThrowIfNull(parameter);
        if(parameter is not FolderViewModel folderViewModel)
            throw new ArgumentException("Parameter must be of type FolderViewModel", nameof(parameter));
        await Load(folderViewModel);
    }

    [RelayCommand]
    private void SelectAllPalettes()
    {
        if(!IsMultiSelectMode)
            throw new InvalidOperationException("Not in multi-select mode.");
        foreach(var palette in Palettes)
        {
            if(!palette.IsSelected)
            {
                palette.IsSelected = true;
                SelectedPalettes.Add(palette);
            }
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach(var palette in SelectedPalettes)
            palette.IsSelected = false;
        SelectedPalettes.Clear();
    }

    [RelayCommand]
    async Task Load(FolderViewModel folder)
    {
        Palettes.Clear();
        SelectedPalettes.Clear();
        IsBusy = true;
        Folder = folder;
        Palettes = new(await _miraiPaletteStorageService.GetPalettesByFolderAsync(folder.Id));
        IsBusy = false;
    }

    partial void OnIsMultiSelectModeChanged(bool value)
    {
        if(value == true)
        {
            PaletteCommand = TogglePaletteSelectionCommand;
        }
        else
        {
            PaletteCommand = NavigateToPaletteCommand;
            ClearSelection();
        }
    }

    [RelayCommand]
    void SetCurrentPalette(PaletteViewModel palette)
    {
        CurrentPalette = palette;
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedPalettes(PaletteViewModel? palette)
    {
        if(palette is null || palette.IsSelected)
        {
            string title, message;
            if(SelectedPalettes.Count == 1)
            {
                title = DeleteConfirmStrings.SinglePalette_Title;
                message = string.Format(DeleteConfirmStrings.SinglePalette_Message, SelectedPalettes[0].Name);
            }
            else
            {
                title = DeleteConfirmStrings.MultiplePalettes_Title;
                message = string.Format(DeleteConfirmStrings.MultiplePalettes_Message, SelectedPalettes.Count);
            }
            var confirmed = await Current.ShowDeleteConfirmDialogAsync(title, message);
            if(!confirmed)
                return;
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePalettesAsync(SelectedPalettes.Select(p => p.Id));
            foreach(var selectedPalette in SelectedPalettes)
                Palettes.Remove(selectedPalette);
            ClearSelection();
            IsBusy = false;
        }
        else
        {
            var confirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.SinglePalette_Title, string.Format(DeleteConfirmStrings.SinglePalette_Message, palette.Name));
            if(!confirmed)
                return;
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePaletteAsync(palette.Id);
            Palettes.Remove(palette);
            ClearSelection();
            IsBusy = false;
        }
        //await Load(Folder);
    }

    [RelayCommand]
    void TogglePaletteSelection(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            DeselectPalette(palette);
        else
            SelectPalette(palette);
    }

    [RelayCommand]
    void NavigateToPalette(PaletteViewModel palette)
    {
        var paras = new Dictionary<string, object>
        {
            { "Palette", palette },
            { "Folder", Folder }
        };
        Navigate(NavigationTarget.Palette, paras);
        ClearSelection();
    }

    [RelayCommand]
    async Task AddPalette()
    {
        IsMultiSelectMode = false;
        var newPalette = new PaletteViewModel()
        {
            Name = Resources.DefaultPaletteTitle,
            Description = string.Empty,
            Colors = [],
            FolderId = Folder.Id
        };
        IsBusy = true;
        await _miraiPaletteStorageService.AddPaletteAsync(newPalette);
        Palettes.Insert(0, newPalette);
        IsBusy = false;
        NavigateToPalette(newPalette);
    }

    [RelayCommand]
    async Task AddPaletteFromFile()
    {
        IsMultiSelectMode = false;
        var path = await Current.PickFileToOpen(OpenFileStrings.PaletteFile_Commit, _paletteFileService.SupportedImportFileExtensions);
        if(path is null)
            return;
        if(!File.Exists(path))
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.ImportPaletteFile_Title, string.Format(ErrorMessages.PathNotExists, path), false);
            return;
        }
        try
        {
            IsBusy = true;
            var palette = await _paletteFileService.Import(path);
            if(palette is null)
            {
                await Current.ShowConfirmDialogAsync(ErrorMessages.ImportPaletteFile_Title, ErrorMessages.ImportPaletteFile_Failed, false);
                return;
            }
            palette.FolderId = Folder.Id;
            await _miraiPaletteStorageService.AddPaletteAsync(palette);
            Palettes.Insert(0, palette);
            NavigateToPalette(palette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.ImportPaletteFile_Title, ErrorMessages.ImportPaletteFile_Error, false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void SelectPalette(PaletteViewModel palette)
    {
        if(palette.IsSelected)
            throw new InvalidOperationException("Palette is already selected.");
        palette.IsSelected = true;
        SelectedPalettes.Add(palette);
    }

    [RelayCommand]
    void DeselectPalette(PaletteViewModel palette)
    {
        if(!palette.IsSelected)
            throw new InvalidOperationException("Palette is not selected.");
        palette.IsSelected = false;
        SelectedPalettes.Remove(palette);
    }

    [RelayCommand]
    async Task NavigateToImagePalette()
    {
        var path = await Current.PickFileToOpen(OpenFileStrings.ImageFile_Commit, ".png", ".jpg", ".jpeg", ".bmp", ".tiff");
        if(path is null)
            return;
        var paras = new Dictionary<string, object>
        {
            { "ImagePath", new Uri(path) },
            { "Folder", Folder }
        };
        Navigate(NavigationTarget.ImagePalette, paras);
        ClearSelection();
    }

    [RelayCommand]
    async Task DeleteFolder()
    {
        if(Folder is null)
            throw new InvalidOperationException("No folder is selected.");
        if(Folder.IsVirtual)
            throw new InvalidOperationException("Cannot delete virtual folder.");
        if(IsBusy)
            return;
        var isConfirmed = await Current.ShowDeleteConfirmDialogAsync("Delete Folder", $"Are you sure you want to delete \"{Folder.Name}\" folder? Be noticed that all of the palettes in the folder will be deleted too.");
        if(!isConfirmed)
            return;
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.DeleteFolderAsync(Folder.Id);
            Messenger.Send(new FolderDeletedMessage(Folder.Id));
        }
        catch(Exception ex)
        {
            await Current.ShowConfirmDialogAsync("Failed to delete folder.", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
