using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Settings;
using MiraiPalette.WinUI.Strings;
using Windows.ApplicationModel;
using Windows.Globalization;

namespace MiraiPalette.WinUI.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public string[] ThemeModeOptions { get; } =
        [
            SettingsPageStrings.AppTheme_System,
            SettingsPageStrings.AppTheme_Light,
            SettingsPageStrings.AppTheme_Dark
        ];

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

    public string ThemeMode
    {
        get => _settingsService.GetValue(ThemeSettings.SettingKey, ThemeSettings.System);
        set
        {
            if(value == ThemeMode)
                return;
            _settingsService.SetValue(ThemeSettings.SettingKey, value);
            OnPropertyChanged(nameof(ThemeMode));
            Current.ApplyThemeModeSetting();
        }
    }

    public string Language
    {
        get => _settingsService.GetValue(LanguageSettings.SettingKey, LanguageSettings.System);
        set
        {
            if(value == Language)
                return;
            _settingsService.SetValue(LanguageSettings.SettingKey, value);
            OnPropertyChanged(nameof(Language));
            _ = LanguageSettings.TryConvertSettingToActual(value, out var language);
            _settingsService.SetValue(LanguageSettings.SettingKey, language);
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
            return $"{version.Major}.{version.Minor}.{version.Build}-winui DEV";
#else
            return $"{version.Major}.{version.Minor}.{version.Build}-winui";
#endif
        }
    }
}
