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

    public static string GetNoPalettesMessage(FolderViewModel folder)
        => folder.Id == FolderViewModel.Unassigned.Id ? MainPageStrings.EmptyUnassignedPalettesMessage : MainPageStrings.EmptyPalettesListMessage;

    /// <summary>
    /// The original name of the folder, used to revert changes if update fails.
    /// </summary>
    private string _originalFolderName = string.Empty;

    partial void OnFolderChanged(FolderViewModel oldValue, FolderViewModel newValue)
    {
        oldValue?.PropertyChanged -= OnFolderPropertyChanged;
        newValue?.PropertyChanged += OnFolderPropertyChanged;
        _originalFolderName = newValue?.Name ?? string.Empty;
    }

    private async void OnFolderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(FolderViewModel.Name))
        {
            if(string.IsNullOrWhiteSpace(Folder.Name))
            {
                Folder.Name = _originalFolderName;
                return;
            }
            EnsureNotBusy();
            try
            {
                IsBusy = true;
                await _miraiPaletteStorageService.UpdateFolderAsync(Folder);
                _originalFolderName = Folder.Name;
            }
            catch(Exception)
            {
                await Current.ShowConfirmDialogAsync(ErrorMessages.UpdateFolder_Title, ErrorMessages.UpdateFolder_Error, false);
                Folder.Name = _originalFolderName;
                return;
            }
            finally
            {
                IsBusy = false;
            }
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
    public partial PaletteViewModel? CurrentPalette { get; set; }

    [ObservableProperty]
    public partial IEnumerable<FolderViewModel>? FoldersForCurrentPaletteToMove { get; set; }

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
        Folder = folder;
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            Palettes = new(await _miraiPaletteStorageService.GetPalettesByFolderAsync(folder.Id));
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.LoadData_Title, ErrorMessages.LoadData_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
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
    void SetCurrentPalette(PaletteViewModel palette) => CurrentPalette = palette;

    [RelayCommand]
    void ResetCurrentPalette() => CurrentPalette = null;

    async Task DeleteSelectedPalettes()
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
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePalettesAsync(SelectedPalettes.Select(p => p.Id));
        }
        catch(Exception)
        {
            var errorTitle = SelectedPalettes.Count == 1 ? ErrorMessages.DeleteSinglePalette_Title : ErrorMessages.DeleteMultiplePalettes_Title;
            var errorMessage = SelectedPalettes.Count == 1 ? ErrorMessages.DeleteSinglePalette_Error : ErrorMessages.DeleteMultiplePalettes_Error;
            await Current.ShowConfirmDialogAsync(errorTitle, errorMessage, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
        foreach(var selectedPalette in SelectedPalettes)
            Palettes.Remove(selectedPalette);
        ClearSelection();
    }

    async Task DeletePalette(PaletteViewModel palette)
    {
        var confirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.SinglePalette_Title, string.Format(DeleteConfirmStrings.SinglePalette_Message, palette.Name));
        if(!confirmed)
            return;
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.DeletePaletteAsync(palette.Id);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.DeleteSinglePalette_Title, ErrorMessages.DeleteSinglePalette_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
        Palettes.Remove(palette);
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedPalettes(PaletteViewModel? palette)
    {
        if(palette is null || palette.IsSelected)
            await DeleteSelectedPalettes();
        else
            await DeletePalette(palette);
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
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            newPalette.Id = await _miraiPaletteStorageService.AddPaletteAsync(newPalette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.CreatePalette_Title, ErrorMessages.CreatePalette_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
        Palettes.Insert(0, newPalette);
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
        EnsureNotBusy();
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
            palette.Id = await _miraiPaletteStorageService.AddPaletteAsync(palette);
            Palettes.Insert(0, palette);
            NavigateToPalette(palette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.ImportPaletteFile_Title, ErrorMessages.ImportPaletteFile_Error, false);
            return;
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
        var isConfirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.Folder_Title, string.Format(DeleteConfirmStrings.Folder_Message, Folder.Name));
        if(!isConfirmed)
            return;
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.DeleteFolderAsync(Folder.Id);
            Messenger.Send(new FolderDeletedMessage(Folder.Id));
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.DeleteFolder_Title, ErrorMessages.DeleteFolder_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    async partial void OnCurrentPaletteChanged(PaletteViewModel? value)
    {
        FoldersForCurrentPaletteToMove = value is null ? null : await GetTargetFoldersToMove(value);
    }

    private async Task<IEnumerable<FolderViewModel>> GetTargetFoldersToMove(PaletteViewModel palette)
    {
        try
        {
            var allFolders = await _miraiPaletteStorageService.GetAllFoldersAsync();
            return allFolders.Where(f => f.Id != palette.FolderId);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.LoadData_Title, ErrorMessages.LoadData_Error, false);
            return [];
        }
    }

    [RelayCommand]
    async Task MovePaletteToFolder(int targetFolderId)
    {
        if(CurrentPalette is null)
            throw new InvalidOperationException("No palette is selected.");
        EnsureNotBusy();
        CurrentPalette.FolderId = targetFolderId;
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.UpdatePaletteAsync(CurrentPalette);
            Palettes.Remove(CurrentPalette);
            CurrentPalette = null;
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.UpdatePalette_Title, ErrorMessages.UpdateFolder_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task RemovePaletteFromFolder()
    {
        if(FolderViewModel.IsVirtualFolder(Folder.Id))
            throw new InvalidOperationException("Cannot remove palette from a virtual folder.");
        if(CurrentPalette is null)
            throw new InvalidOperationException("No palette is selected.");
        EnsureNotBusy();
        CurrentPalette.FolderId = FolderViewModel.Unassigned.Id;
        try
        {
            IsBusy = true;
            await _miraiPaletteStorageService.UpdatePaletteAsync(CurrentPalette);
            Palettes.Remove(CurrentPalette);
            CurrentPalette = null;
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.UpdatePalette_Title, ErrorMessages.UpdateFolder_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    async Task MoveSelectedPalettesToFolder(int targetFolderId)
    {
        if(SelectedPalettes.Count == 0)
            throw new InvalidOperationException("No palettes are selected.");

    }
}
