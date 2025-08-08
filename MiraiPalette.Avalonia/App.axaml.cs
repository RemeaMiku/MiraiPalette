using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MiraiPalette.Avalonia.ViewModels;
using MiraiPalette.Avalonia.Views;

namespace MiraiPalette.Avalonia;

public partial class App : Application
{
    public App()
    {
        Services = ConfigureServices();
    }

    public static new App Current => (Application.Current as App)!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public IServiceProvider Services { get; init; }

    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<PalettesPageViewModel>()
            .AddSingleton<ImagePalettePageViewModel>()
            .AddSingleton<MainWindow>()
            .AddSingleton<PalettesPage>()
            .AddSingleton<ImagePalettePage>();
        return services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 如果使用 CommunityToolkit，则需要用下面一行移除 Avalonia 数据验证。
        // 如果没有这一行，数据验证将会在 Avalonia 和 CommunityToolkit 中重复。
        BindingPlugins.DataValidators.RemoveAt(0);
        var mainWindow = Services.GetRequiredService<MainWindow>();
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }
        else if(ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = mainWindow;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
