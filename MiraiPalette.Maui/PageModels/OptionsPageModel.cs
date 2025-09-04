using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Options;
using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.PageModels;

public partial class OptionsPageModel(IPreferences preferences) : ObservableObject
{
    readonly IPreferences _preferences = preferences;

    [ObservableProperty]
    public partial int SelectedThemeIndex { get; set; } = -1;

    public string[] ThemeOptionItems { get; } = [StringResource.FollowSystem, StringResource.LightMode, StringResource.DarkMode];

    private readonly string[] _themeOptions = [ThemeOptions.System, ThemeOptions.Light, ThemeOptions.Dark];

    [RelayCommand]
    private void Load()
    {
        var theme = _preferences.Get(ThemeOptions.Key, ThemeOptions.Default);
        var themeIndex = _themeOptions.IndexOf(theme);
        SelectedThemeIndex = themeIndex;
    }

    partial void OnSelectedThemeIndexChanged(int value)
    {
        var theme = _themeOptions[SelectedThemeIndex];
        _preferences.Set(ThemeOptions.Key, theme);
        App.Current.UserAppTheme = ThemeOptions.ToAppTheme(theme);
    }
}