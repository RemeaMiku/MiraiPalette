using System;
using Microsoft.UI.Xaml.Data;
using MiraiPalette.WinUI.Settings;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.Converters;

public partial class LanguageCodeToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value is not string languageCode
            ? throw new ArgumentException("Value must be a string.", nameof(value))
            : languageCode switch
            {
                LanguageSettings.System => SettingsPageStrings.Language_System,
                LanguageSettings.zhCN => SettingsPageStrings.Language_zhCN,
                LanguageSettings.enUS => SettingsPageStrings.Language_enUS,
                LanguageSettings.jaJP => SettingsPageStrings.Language_jaJP,
                _ => throw new ArgumentException("Unsupported language code.", nameof(value))
            };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value is not string languageName
            ? throw new ArgumentException("Value must be a string.", nameof(value))
            : languageName == SettingsPageStrings.Language_System
            ? LanguageSettings.System
            : languageName == SettingsPageStrings.Language_zhCN
            ? LanguageSettings.zhCN
            : languageName == SettingsPageStrings.Language_enUS
            ? LanguageSettings.enUS
            : languageName == SettingsPageStrings.Language_jaJP
            ? (object)LanguageSettings.jaJP
            : throw new ArgumentException("Unsupported language name.", nameof(value));
    }
}
