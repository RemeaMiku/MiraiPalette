using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Models;
using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Services;

namespace MiraiPalette.Maui.PageModels;

public partial class PaletteDetailPageModel(IPaletteService paletteService) : ObservableObject, IQueryAttributable
{
    [RelayCommand]
    private async Task Update()
    {
        await _paletteService.UpdatePaletteAsync(Palette);
        await Load();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        CloseColorDetail();
        ShellFlyoutHelper.RestoreFlyout();
        await Shell.Current.GoToAsync(ShellRoutes.GoBack);
    }

    private readonly IPaletteService _paletteService = paletteService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescriptionButtonText), nameof(PaletteNameButtonTooltip))]
    public partial MiraiPaletteModel Palette { get; set; } = new()
    {
        Name = Constants.DefaultPaletteName,
    };

    private static readonly string _descriptionPlaceholder = $"({StringResource.AddDescriptionsForPalette})";

    public string DescriptionButtonText => string.IsNullOrWhiteSpace(Palette.Description) ? _descriptionPlaceholder : Palette.Description;

    public string PaletteNameButtonTooltip => $"{Palette.Name} ({StringResource.ClickToRename})";

    public string PaletteDescriptionButtonTooltip { get; } = StringResource.ClickToEditDescription;

    public string AddNewColorText { get; } = StringResource.AddNewColor;

    public string DeletePaletteText { get; } = StringResource.DeletePalette;

    public string CurrentColorNameButtonTooltip => $"{CurrentColor.Name} ({StringResource.ClickToRename})";

    public string SaveColorText { get; } = StringResource.Save;

    public string RemoveColorText { get; } = StringResource.Remove;

    public string CloseColorDetailText { get; } = StringResource.CloseColorDetailPanel;

    public string CopyToClipboardText { get; } = StringResource.CopyToClipboard;

    [ObservableProperty]
    public partial MiraiColorModel? SelectedExistColor { get; set; }

    [ObservableProperty]
    public partial bool IsColorDetailOpen { get; set; } = false;

    public MiraiColorModel CurrentColor { get; } = new()
    {
        Name = Constants.DefaultColorName,
        Color = Color.FromArgb(Constants.DefaultColorAsHex),
    };

    [RelayCommand]
    private void AddNewColor()
    {
        OpenColorDetail(default);
    }

    [RelayCommand]
    private async Task DeletePalette()
    {
        var isYes = await Shell.Current.DisplayAlert(StringResource.DeletePalette, string.Format(StringResource.DeletePaletteDialogMessage, Palette.Name), StringResource.Confirm, StringResource.Cancel);
        if(!isYes)
            return;
        await _paletteService.DeletePaletteAsync(Palette.Id);
        GoBackCommand.Execute(null);
    }

    [RelayCommand]
    private async Task DeleteSelectedColor()
    {
        if(SelectedExistColor is null)
            throw new InvalidOperationException("Selected color is null");
        var isYes = await Shell.Current.DisplayAlert(StringResource.DeleteColor, string.Format(StringResource.RemoveColorDialogMessage, CurrentColor.Name), StringResource.Confirm, StringResource.Cancel);
        if(!isYes)
            return;
        await _paletteService.DeleteColorAsync(SelectedExistColor.Id);
        await Load();
        CloseColorDetail();
    }

    private void ClearColorDetail()
    {
        CurrentColor.Name = Constants.DefaultColorName;
        CurrentColor.Color = Color.FromArgb(Constants.DefaultColorAsHex);
    }

    private void ClearSelection()
    {
        if(SelectedExistColor is not null)
        {
            SelectedExistColor.IsSelected = false;
            SelectedExistColor = default;
        }
    }

    [RelayCommand]
    private void OpenColorDetail(MiraiColorModel? color)
    {
        ClearSelection();
        SelectedExistColor = color;
        if(SelectedExistColor is null)
            ClearColorDetail();
        else
        {
            SelectedExistColor.IsSelected = true;
            CurrentColor.Name = SelectedExistColor.Name;
            CurrentColor.Color = SelectedExistColor.Color;
        }
        IsColorDetailOpen = true;
    }

    [RelayCommand]
    private void CloseColorDetail()
    {
        IsColorDetailOpen = false;
        ClearColorDetail();
        ClearSelection();
    }

    [RelayCommand]
    private async Task SaveColorDetail()
    {
        if(SelectedExistColor is null)
        {
            await _paletteService.InsertColorAsync(Palette.Id, new MiraiColorModel
            {
                Name = CurrentColor.Name,
                Color = CurrentColor.Color,
            });
        }
        else
        {
            await _paletteService.UpdateColorAsync(new MiraiColorModel
            {
                Id = SelectedExistColor.Id,
                Name = CurrentColor.Name,
                Color = CurrentColor.Color,
            });
        }
        Unload();
        await Load();
    }

    private int _paletteId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _paletteId = query.TryGetValue(nameof(MiraiPaletteModel.Id), out var idObj) && idObj is int id
            ? id
            : throw new ArgumentException("Palette Id not found in query parameters", nameof(query));
    }



    [RelayCommand]
    private async Task Load()
    {
        ShellFlyoutHelper.DisableFlyout();
        var palette = await _paletteService.SelectPaletteAsync(_paletteId);
        if(palette is null)
        {
            await Shell.Current.DisplayAlert("Error", "Palette not found", "OK");
            await Shell.Current.GoToAsync(ShellRoutes.GoBack);
            return;
        }
        Palette = palette;
    }

    [RelayCommand]
    private void Unload()
    {
        CloseColorDetail();
        Palette = new MiraiPaletteModel
        {
            Name = Constants.DefaultPaletteName,
        };
    }
}