using MiraiPalette.Maui.Essentials;
using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Resources.Styles.Themes;

namespace MiraiPalette.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyTheme(e.RequestedTheme);
    }

    public void ApplyTheme(AppTheme theme)
    {
        var dictionaries = Resources.MergedDictionaries;
        var currentThemeDictionary = dictionaries.FirstOrDefault(d => d is not null && d.Source?.OriginalString.Contains("Themes") == true);
        dictionaries.Remove(currentThemeDictionary);
        ResourceDictionary newThemeDictionary = theme switch
        {
            AppTheme.Light => new LightTheme(),
            AppTheme.Dark => new DarkTheme(),
            _ => new DarkTheme()
        };
        dictionaries.Add(newThemeDictionary);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var appShell = new AppShell();
        var window = new Window(appShell)
        {
            MinimumWidth = 400,
            MinimumHeight = 600,
            Title = StringResource.MainWindowTitle
        };
        ShellFlyoutHelper.RegisterListener(window);
        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        OnRequestedThemeChanged(this, new AppThemeChangedEventArgs(RequestedTheme));
        RequestedThemeChanged += OnRequestedThemeChanged;
    }
}