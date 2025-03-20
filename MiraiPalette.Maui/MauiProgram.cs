using CommunityToolkit.Maui;
using MauiIcons.Core;
using MauiIcons.Fluent;
using Microsoft.Extensions.Logging;
using MiraiPalette.Maui.PageModels;
using MiraiPalette.Maui.Pages;
using MiraiPalette.Maui.Services;
using MiraiPalette.Maui.Services.Local;

namespace MiraiPalette.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
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
            .AddSingleton<IPaletteRepositoryService, JsonPaletteRepositoryService>()
            .AddSingleton<MainPageModel>()
            .AddTransientWithShellRoute<PaletteDetailPage, PaletteDetailPageModel>(ShellRoutes.PaletteDetailPage);
        return builder.Build();
    }
}