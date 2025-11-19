using System;
using Microsoft.UI.Xaml.Data;
using MiraiPalette.WinUI.Settings;
using MiraiPalette.WinUI.Strings;

namespace MiraiPalette.WinUI.Converters;

public partial class ThemeValueToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not string themeValue
            ? throw new ArgumentException("Value must be a string", nameof(value))
            : themeValue switch
            {
                ThemeSettings.System => SettingsPageStrings.AppTheme_System,
                ThemeSettings.Light => SettingsPageStrings.AppTheme_Light,
                ThemeSettings.Dark => SettingsPageStrings.AppTheme_Dark,
                _ => throw new ArgumentException("Invalid theme value", nameof(value)),
            };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is not string themeName
            ? throw new ArgumentException("Value must be a string", nameof(value))
            : themeName == SettingsPageStrings.AppTheme_System
            ? ThemeSettings.System
            : themeName == SettingsPageStrings.AppTheme_Light
            ? ThemeSettings.Light
            : themeName == SettingsPageStrings.AppTheme_Dark
            ? (object)ThemeSettings.Dark
            : throw new ArgumentException("Invalid theme value", nameof(value));
    }
}
