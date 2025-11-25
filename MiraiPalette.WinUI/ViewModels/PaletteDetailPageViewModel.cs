using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using MiraiPalette.WinUI.Essentials;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Strings;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class PaletteDetailPageViewModel : PageViewModelBase
{
    public PaletteDetailPageViewModel(IMiraiPaletteStorageService miraiPaletteStorageService, IPaletteFileService paletteFileService)
    {
        _miraiPaletteStorageService = miraiPaletteStorageService;
        _paletteFileService = paletteFileService;
    }

    partial void OnPaletteChanged(PaletteViewModel value)
    {
        value.Colors.CollectionChanged += PaletteColors_CollectionChanged;
    }

    private void PaletteColors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdatePaletteCommand.Execute(null);
    }

    [RelayCommand]
    async Task UpdatePalette()
    {
        if(IsBusy)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.AppIsBusy_Title, ErrorMessages.AppIsBusy_Retry, false);
            return;
        }
        IsBusy = true;
        try
        {
            await _miraiPaletteStorageService.UpdatePaletteAsync(Palette);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.UpdatePalette_Title, ErrorMessages.UpdatePalette_Error);
        }
        IsBusy = false;
    }

    private readonly IMiraiPaletteStorageService _miraiPaletteStorageService;

    private readonly IPaletteFileService _paletteFileService;

    [ObservableProperty]
    public partial PaletteViewModel Palette { get; private set; } = null!;

    [ObservableProperty]
    public partial bool IsMultiSelectionEnabled { get; set; } = false;

    [ObservableProperty]
    public partial bool IsEditingColor { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedColors))]
    public partial ColorViewModel? SelectedColor { get; set; }

    Color _colorBefore = Colors.Transparent;

    public bool HasSelectedColors => SelectedColor is not null;

    [RelayCommand]
    public async Task Load(PaletteViewModel palette)
    {
        Palette = palette;
        palette.PropertyChanged += Palette_PropertyChanged;
        foreach(var color in palette.Colors)
            color.PropertyChanged += Color_PropertyChanged;
    }

    private void Color_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ColorViewModel.Name))
            UpdatePaletteCommand.Execute(null);
    }

    private async void Palette_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is nameof(PaletteViewModel.Title) && string.IsNullOrWhiteSpace(Palette.Title))
        {
            var title = (await _miraiPaletteStorageService.GetPaletteAsync(Palette.Id))!.Title;
            Palette.Title = title;
            return;
        }
        if(e.PropertyName is nameof(PaletteViewModel.Title) or nameof(PaletteViewModel.Description))
            UpdatePaletteCommand.Execute(null);
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
        {
            IsEditingColor = false;
        }
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
        await UpdatePalette();
        SelectedColor = Palette.Colors[0];
    }

    [RelayCommand]
    async Task ExportPalette()
    {
        var path = await Current.PickPathToSave(Palette.Title, SaveFileStrings.PaletteFile_Commit, _paletteFileService.SupportedExportFileTypes);
        if(path is null)
            return;
        try
        {
            IsBusy = true;
            await _paletteFileService.Export(Palette, path);
        }
        catch(Exception)
        {
            await Current.ShowConfirmDialogAsync(ErrorMessages.ExportPaletteFile_Title, ErrorMessages.ExportPaletteFile_Error);
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
        var isConfirmed = await Current.ShowDeleteConfirmDialogAsync(DeleteConfirmStrings.SinglePalette_Title, string.Format(DeleteConfirmStrings.SinglePalette_Message, Palette.Title));
        if(!isConfirmed)
            return;
        IsBusy = true;
        await _miraiPaletteStorageService.DeletePaletteAsync(Palette.Id);
        IsBusy = false;
        Current.NavigateTo(NavigationTarget.Back);
    }
}