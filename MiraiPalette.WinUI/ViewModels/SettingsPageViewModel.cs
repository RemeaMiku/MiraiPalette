using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using MiraiPalette.WinUI.Services;

namespace MiraiPalette.WinUI.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public SettingsPageViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string ThemeMode
    {
        get => _settingsService.GetValue(nameof(ThemeMode), nameof(ElementTheme.Default));
        set
        {
            if(value == ThemeMode)
                return;
            _settingsService.SetValue(nameof(ThemeMode), value);
            OnPropertyChanged(nameof(ThemeMode));
            Current.ApplyThemeModeSetting();
        }
    }
}
