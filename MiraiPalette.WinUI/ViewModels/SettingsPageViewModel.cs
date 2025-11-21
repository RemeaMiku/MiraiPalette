using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
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

    public OrderedDictionary<ElementTheme, string> ThemeModeOptions { get; } =
        new()
        {
            { ElementTheme.Default, SettingsPageStrings.AppTheme_System },
            { ElementTheme.Light, SettingsPageStrings.AppTheme_Light },
            { ElementTheme.Dark, SettingsPageStrings.AppTheme_Dark }
        };

    public OrderedDictionary<NavigationViewPaneDisplayMode, string> NavigationStyleOptions { get; } =
        new()
        {
            { NavigationViewPaneDisplayMode.Left, SettingsPageStrings.NavigationStyle_Left },
            { NavigationViewPaneDisplayMode.Top, SettingsPageStrings.NavigationStyle_Top }
        };

    public OrderedDictionary<string, string> LanguageOptions { get; } =
       new()
       {
            { LanguageSettings.System, SettingsPageStrings.Language_System },
            { LanguageSettings.zhCN, SettingsPageStrings.Language_zhCN },
            { LanguageSettings.enUS, SettingsPageStrings.Language_enUS },
            { LanguageSettings.jaJP, SettingsPageStrings.Language_jaJP }
       };

    public SettingsPageViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public int ThemeModeIndex
    {
        get => ThemeModeOptions.IndexOf(_settingsService.GetValue(SettingKeys.Theme, ElementTheme.Default));
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
        get => NavigationStyleOptions.IndexOf(_settingsService.GetValue(SettingKeys.NavigationStyle, NavigationViewPaneDisplayMode.Left));
        set
        {
            if(value == NavigationStyleIndex)
                return;
            _settingsService.SetValue(SettingKeys.NavigationStyle, NavigationStyleOptions.GetAt(value).Key);
            OnPropertyChanged(nameof(NavigationStyleIndex));
            Current.ApplyNavigationStyleSetting();
        }
    }

    public int LanguageIndex
    {
        get => LanguageOptions.IndexOf(_settingsService.GetValue(SettingKeys.Language, LanguageSettings.System));
        set
        {
            if(value == LanguageIndex)
                return;
            var setting = LanguageOptions.GetAt(value).Key;
            _settingsService.SetValue(SettingKeys.Language, setting);
            OnPropertyChanged(nameof(LanguageIndex));
            LanguageSettings.TryConvertSettingToActual(setting, out var language);
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
