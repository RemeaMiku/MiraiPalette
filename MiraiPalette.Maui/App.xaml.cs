using MiraiPalette.Maui.Resources.Globalization;
using MiraiPalette.Maui.Resources.Styles.Themes;

namespace MiraiPalette.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    private void OnCurrentRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if(e.RequestedTheme == AppTheme.Unspecified)
            return;
        var dictionaries = Current!.Resources.MergedDictionaries;
        var currentThemeDictionary = dictionaries.FirstOrDefault(d => d is not null && d.Source?.OriginalString.Contains("Themes") == true);
        dictionaries.Remove(currentThemeDictionary);
        ResourceDictionary newThemeDictionary = e.RequestedTheme switch
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
            MinimumWidth = 350,
            MinimumHeight = 600,
            Title = StringResource.MainWindowTitle
        };
        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        OnCurrentRequestedThemeChanged(this, new AppThemeChangedEventArgs(Current!.RequestedTheme));
        Current!.RequestedThemeChanged += OnCurrentRequestedThemeChanged;
    }
}