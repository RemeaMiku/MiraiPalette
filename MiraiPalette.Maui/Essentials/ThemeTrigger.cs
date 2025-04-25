namespace MiraiPalette.Maui.Essentials;

public static class ThemeTrigger
{
    static ThemeTrigger()
    {
        Application.Current!.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    private static readonly Dictionary<AppTheme, List<Action>> _actions = new()
    {
            { AppTheme.Dark , []},
            { AppTheme.Light , []},
            { AppTheme.Unspecified , []}
    };

    public static void Register(AppTheme theme, Action onThemeChanged)
    {
        _actions[theme].Add(onThemeChanged);
    }

    private static void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        var theme = Application.Current!.RequestedTheme;
        _actions[theme].ForEach(a => a());
    }
}
