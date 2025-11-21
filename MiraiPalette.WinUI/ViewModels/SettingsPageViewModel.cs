using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Settings;
using MiraiPalette.WinUI.Strings;
using Windows.ApplicationModel;
using Windows.Globalization;

namespace MiraiPalette.WinUI.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public OrderedDictionary<string, string> ThemeModeOptions { get; } =
        new()
        {
            { ThemeSettings.System, SettingsPageStrings.AppTheme_System },
            { ThemeSettings.Light, SettingsPageStrings.AppTheme_Light },
            { ThemeSettings.Dark, SettingsPageStrings.AppTheme_Dark }
        };

    public OrderedDictionary<NavigationViewPaneDisplayMode, string> NavigationStyleOptions { get; } =
        new()
        {
            { NavigationViewPaneDisplayMode.Left, SettingsPageStrings.NavigationStyle_Left },
            { NavigationViewPaneDisplayMode.Top, SettingsPageStrings.NavigationStyle_Top }
        };

    public string[] LanguageOptions { get; } =
        [
            SettingsPageStrings.Language_System,
            SettingsPageStrings.Language_zhCN,
            SettingsPageStrings.Language_enUS,
            SettingsPageStrings.Language_jaJP
        ];

    public SettingsPageViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public int ThemeModeIndex
    {
        get => ThemeModeOptions.IndexOf(_settingsService.GetValue(SettingKeys.Theme, ThemeSettings.System));
        set
        {
            if(value == ThemeModeIndex)
                return;
            _settingsService.SetValue(SettingKeys.Theme, ThemeModeOptions.GetAt(value).Key);
            OnPropertyChanged(nameof(ThemeModeIndex));
            Current.ApplyThemeModeSetting();
        }
    }

    public int NavigationStyleIndex
    {
        get => NavigationStyleOptions.IndexOf(
            (NavigationViewPaneDisplayMode)_settingsService.GetValue(SettingKeys.NavigationStyle, (int)NavigationViewPaneDisplayMode.Left));
        set
        {
            if(value == NavigationStyleIndex)
                return;
            var settingValue = NavigationStyleOptions.GetAt(value).Key;
            _settingsService.SetValue(SettingKeys.NavigationStyle, settingValue);
            OnPropertyChanged(nameof(NavigationStyleIndex));
            Current.ApplyNavigationStyleSetting();
        }
    }

    public string Language
    {
        get => _settingsService.GetValue(SettingKeys.Language, LanguageSettings.System);
        set
        {
            if(value == Language)
                return;
            _settingsService.SetValue(SettingKeys.Language, value);
            OnPropertyChanged(nameof(Language));
            _ = LanguageSettings.TryConvertSettingToActual(value, out var language);
            ApplicationLanguages.PrimaryLanguageOverride = language;
            NeedRestartForChanges = CultureInfo.DefaultThreadCurrentUICulture?.Name != language;
        }
    }

    [ObservableProperty]
    public partial bool NeedRestartForChanges { get; set; }

    public string Version
    {
        get
        {
            var version = Package.Current.Id.Version;
#if DEBUG
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision} Dev";
#else
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
#endif
        }
    }
}
