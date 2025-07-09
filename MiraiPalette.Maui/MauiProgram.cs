using CommunityToolkit.Maui;
using MauiIcons.Core;
using MauiIcons.Fluent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MiraiPalette.Maui.PageModels;
using MiraiPalette.Maui.Pages;
using MiraiPalette.Maui.Services;
using MiraiPalette.Maui.Services.Local;


[assembly: RootNamespace("MiraiPalette.Maui")]
namespace MiraiPalette.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit(options => options.SetShouldEnableSnackbarOnWindows(true))
            .UseFluentMauiIcons()
            .UseMauiIconsCore(x =>
            {
                x.SetDefaultIconSize(24);
                x.SetDefaultFontOverride(true);
                x.SetDefaultIconAutoScaling(true);
            })
            .UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
        });
#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddLocalization(options =>
            options.ResourcesPath = "Resources\\Globalization")            
            .AddSingleton<IPaletteRepositoryService, JsonPaletteRepositoryService>()
            .AddSingletonWithShellRoute<MainPage, MainPageModel>(ShellRoutes.MainPage)
            .AddSingletonWithShellRoute<ImagePalettePage, ImagePalettePageModel>(ShellRoutes.ImagePalettePage)
            .AddSingletonWithShellRoute<OptionsPage, OptionsPageModel>(ShellRoutes.OptionsPage)
            .AddTransientWithShellRoute<PaletteDetailPage, PaletteDetailPageModel>(ShellRoutes.PaletteDetailPage);
        return builder.Build();
    }
}