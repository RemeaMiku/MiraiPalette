namespace MiraiPalette.Maui.Options;

public static class ThemeOptions
{
    public const string Key = "Theme";

    public const string System = nameof(System);

    public const string Dark = nameof(Dark);

    public const string Light = nameof(Light);

    public const string Default = System;

    public static AppTheme ToAppTheme(string theme) => theme switch
    {
        Dark => AppTheme.Dark,
        Light => AppTheme.Light,
        System => AppTheme.Unspecified,
        _ => throw new ArgumentException(theme)
    };
}
