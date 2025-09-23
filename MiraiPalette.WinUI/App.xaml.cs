using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Services.Local;
using MiraiPalette.WinUI.ViewModels;
using MiraiPalette.WinUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    public new static App Current => (App)Application.Current;

    public ServiceProvider Services { get; } = ConfigureServices();

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection()
            .AddSingleton<IPaletteDataService, MiraiPaletteDataFilePaletteDataService>()
            .AddSingleton<MainPageViewModel>()
            .AddSingleton<MainPage>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<MainWindow>();
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
