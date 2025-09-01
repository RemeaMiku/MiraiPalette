using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiraiPalette.Maui.Options;
using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.PageModels;

public partial class OptionsPageModel(IPreferences preferences) : ObservableObject
{
    readonly IPreferences _preferences = preferences;

    [ObservableProperty]
    public partial int SelectedThemeIndex { get; set; }

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
        if(theme == ThemeOptions.System)
        {
            Application.Current!.UserAppTheme = AppTheme.Unspecified;
        }
        else
        {
            AppTheme appTheme;
            switch(theme)
            {
                case ThemeOptions.Dark:
                    appTheme = AppTheme.Dark;
                    break;
                case ThemeOptions.Light:
                    appTheme = AppTheme.Light;
                    break;
                default:
                    throw new NotImplementedException();
            }
            Application.Current?.UserAppTheme = appTheme;
        }
    }
}