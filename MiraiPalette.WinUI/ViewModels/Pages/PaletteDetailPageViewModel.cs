using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteDetailPageViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IPaletteFileService paletteFileService, IMessenger messenger) : PageViewModelBase(messenger)
{

    [RelayCommand]
    async Task UpdatePalette()
    {
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            await miraiPaletteStorageService.UpdatePaletteAsync(Palette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.UpdatePalette_Title, ErrorMessages.UpdatePalette_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty]
    public partial PaletteViewModel Palette { get; private set; } = null!;

    /// <summary>
    /// The original name of the palette when loaded.
    /// </summary>
    private string _originalPaletteName = string.Empty;

    [ObservableProperty]
    public partial FolderViewModel? Folder { get; private set; }

    [ObservableProperty]
    public partial bool IsMultiSelectionEnabled { get; set; } = false;

    [ObservableProperty]
    public partial bool IsEditingColor { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedColors))]
    public partial ColorViewModel? SelectedColor { get; set; }

    Color _colorBefore = Colors.Transparent;

    public bool HasSelectedColors => SelectedColor is not null;

    public override async void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        ArgumentNullException.ThrowIfNull(parameter);
        switch(parameter)
        {
            case PaletteViewModel palettePara:
                await Load(palettePara);
                Folder = default;
                break;
            case Dictionary<string, object> dict:
                if(dict.TryGetValue("Palette", out var paletteObj) && paletteObj is PaletteViewModel palette)
                    await Load(palette);
                if(dict.TryGetValue("Folder", out var folderObj) && folderObj is FolderViewModel folder)
                    Folder = folder;
                break;
            default:
                break;
        }
    }

    partial void OnPaletteChanged(PaletteViewModel oldValue, PaletteViewModel newValue)
    {
        if(oldValue is not null)
        {
            oldValue.PropertyChanged -= Palette_PropertyChanged;
            oldValue.Colors.CollectionChanged -= Colors_CollectionChanged;
            foreach(var color in oldValue.Colors)
                color.PropertyChanged -= Color_PropertyChanged;
        }

        if(newValue is not null)
        {
            newValue.PropertyChanged += Palette_PropertyChanged;
            newValue.Colors.CollectionChanged += Colors_CollectionChanged;
            foreach(var color in newValue.Colors)
                color.PropertyChanged += Color_PropertyChanged;

            _originalPaletteName = newValue.Name;
        }
    }

    private async void Colors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if(e.Action == NotifyCollectionChangedAction.Add)
            await UpdatePalette();
    }

    [RelayCommand]
    public async Task Load(PaletteViewModel palette)
    {
        Palette = palette;
    }

    private async void Color_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ColorViewModel.Name))
            await UpdatePalette();
    }

    private async void Palette_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is nameof(PaletteViewModel.Name))
        {
            if(string.IsNullOrWhiteSpace(Palette.Name))
            {
                Palette.Name = _originalPaletteName;
                return;
            }
            await UpdatePalette();
            _originalPaletteName = Palette.Name;
        }
        if(e.PropertyName is nameof(PaletteViewModel.Description))
            await UpdatePalette();
    }

    [RelayCommand]
    public void UpdateColorSelection(IList<object> selection)
    {
        selection ??= Array.Empty<object>();
        foreach(var color in Palette.Colors)
            color.IsSelected = selection.Contains(color);
        if(selection.Count == 1 && !IsMultiSelectionEnabled)
        {
            EditColor();
            IsEditingColor = true;
        }
        else
            IsEditingColor = false;
    }

    [RelayCommand]
    static void CopyColorToClipboard(ColorViewModel color)
    {
        var package = new DataPackage();
        package.SetText(color.Hex);
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    async Task AddColor()
    {
        Palette.Colors.Insert(0, new ColorViewModel()
        {
            Name = $"{Resources.DefaultColorName} {Palette.Colors.Count + 1}",
            Color = Colors.White
        });
        //await UpdatePalette();
        SelectedColor = Palette.Colors[0];
    }

    [RelayCommand]
    async Task ExportPalette()
    {
        var path = await Current.PickPathToSave(Palette.Name, SaveFileStrings.PaletteFile_Commit, paletteFileService.SupportedExportFileTypes);
        if(path is null)
            return;
        EnsureNotBusy();
        try
        {
            IsBusy = true;
            await paletteFileService.Export(Palette, path);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.ExportPaletteFile_Title, ErrorMessages.ExportPaletteFile_Error, false);
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void EditColor(ColorViewModel? color = default)
    {
        if(color is null && SelectedColor is null)
            throw new ArgumentNullException(nameof(color));
        if(color is not null)
            SelectedColor = color;
        _colorBefore = SelectedColor!.Color;
    }

    [RelayCommand]
    void ResetEditingColor()
    {
        if(SelectedColor is null)
            return;
        SelectedColor.Color = _colorBefore;
    }

    [RelayCommand]
    async Task SaveEditingColor()
    {
        if(SelectedColor is null)
            return;
        if(SelectedColor.Color != _colorBefore)
            await UpdatePalette();
        SelectedColor = null;
        _colorBefore = Colors.Transparent;
    }

    partial void OnSelectedColorChanging(ColorViewModel? oldValue, ColorViewModel? newValue)
    {
        if(IsEditingColor && oldValue is not null && newValue is not null)
            oldValue.Color = _colorBefore;
    }

    [RelayCommand]
    async Task ExitColorEditing()
    {
        if(SelectedColor is null)
            return;
        if(SelectedColor.Color != _colorBefore)
        {
            var isConfirmed = await Current.ShowConfirmDialogAsync(
                SaveChangesStrings.ColorValue_Title,
                string.Format(SaveChangesStrings.ColorValue_Message, SelectedColor.Name),
                true,
                SaveChangesStrings.ColorValue_Commit);
            if(isConfirmed)
                await UpdatePalette();
            else
                ResetEditingColor();
        }
        SelectedColor = null;
    }

    [RelayCommand]
    async Task DeleteCurrentOrSelectedColors(ColorViewModel? currentColor)
    {
        if(currentColor is null || currentColor.IsSelected)
        {
            var selectedColors = Palette.Colors.Where(c => c.IsSelected).ToArray();
            if(selectedColors.Length == 0)
                return;
            string title, message;
            if(selectedColors.Length == 1)
            {
                title = DeleteConfirmStrings.SingleColor_Title;
                message = string.Format(DeleteConfirmStrings.SingleColor_Message, selectedColors[0].Name);
            }
            else
            {
                title = DeleteConfirmStrings.MultipleColors_Title;
                message = string.Format(DeleteConfirmStrings.MultipleColors_Message, selectedColors.Length);
            }
            var isConfirmed = await Current.ShowDeleteConfirmDialogAsync(title, message);
            if(!isConfirmed)
                return;
            foreach(var color in selectedColors)
                Palette.Colors.Remove(color);
            await UpdatePalette();
        }
        else
        {
            var isConfirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.SingleColor_Title, string.Format(DeleteConfirmStrings.SingleColor_Message, currentColor.Name));
            if(!isConfirmed)
                return;
            Palette.Colors.Remove(currentColor);
            await UpdatePalette();
        }
    }

    [RelayCommand]
    async Task DeletePalette()
    {
        EnsureNotBusy();
        var isConfirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.SinglePalette_Title, string.Format(DeleteConfirmStrings.SinglePalette_Message, Palette.Name));
        if(!isConfirmed)
            return;
        try
        {
            IsBusy = true;
            await miraiPaletteStorageService.DeletePaletteAsync(Palette.Id);
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
        ReturnToFolder();
    }

    [RelayCommand]
    void ReturnToFolder()
    {
        if(IsBusy)
            return;
        if(Folder is null)
            throw new InvalidOperationException("Cannot return to folder because Folder is null.");
        Navigate(NavigationTarget.Back, Folder);
    }
}