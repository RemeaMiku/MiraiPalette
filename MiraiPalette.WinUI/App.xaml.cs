using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Services.Impl;
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
    public MainWindow MainWindow => Services.GetRequiredService<MainWindow>();

    public new static App Current => (App)Application.Current;

    public ServiceProvider Services { get; } = ConfigureServices();

    public ElementTheme ThemeMode
    {
        get
        {
            var frameworkElement = MainWindow.Content as FrameworkElement;
            return frameworkElement?.RequestedTheme ?? ElementTheme.Default;
        }
        private set
        {
            var frameworkElement = MainWindow.Content as FrameworkElement;
            frameworkElement?.RequestedTheme = value;
        }
    }

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection()
            .AddSingleton<IMiraiPaletteStorageService, MiraiPaletteDataFileStorageService>()
            .AddSingleton<IPaletteFileService, PaletteFileService>()
            .AddSingleton<ISettingsService, LocalSettingsService>()
            .AddTransient<PaletteDetailPageViewModel>()
            .AddTransient<ImagePalettePageViewModel>()
            .AddSingleton<MainPageViewModel>()
            .AddSingleton<SettingsPageViewModel>()
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
        base.OnLaunched(args);
        MainWindow.Activate();
        ApplyThemeModeSetting();
    }

    public void ApplyThemeModeSetting()
    {
        var settingsService = Services.GetRequiredService<ISettingsService>();
        var themeModeStr = settingsService.GetValue(nameof(ThemeMode), nameof(ElementTheme.Default));
        if(!Enum.TryParse<ElementTheme>(themeModeStr, out var themeMode))
        {
            settingsService.SetValue(nameof(ThemeMode), nameof(ElementTheme.Default));
            return;
        }
        ThemeMode = themeMode;
    }

    public void NavigateTo(NavigationTarget target, object? parameter = null)
    {
        var frame = MainWindow.NavigationFrame;
        switch(target)
        {
            case NavigationTarget.Back:
                if(frame.CanGoBack)
                    frame.GoBack();
                break;
            case NavigationTarget.Main:
                frame.Navigate(typeof(MainPage), parameter);
                break;
            case NavigationTarget.Palette:
                frame.Navigate(typeof(PaletteDetailPage), parameter);
                break;
            case NavigationTarget.ImagePalette:
                frame.Navigate(typeof(ImagePalettePage), parameter);
                break;
            case NavigationTarget.Settings:
                frame.Navigate(typeof(SettingsPage), parameter);
                break;
            default:
                break;
        }
    }

    public async Task<bool> ShowConfirmDialogAsync(string title, string content, bool showCancelButton = true)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = "确定",
            SecondaryButtonText = "取消",
            IsSecondaryButtonEnabled = showCancelButton,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = MainWindow.Content.XamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<string?> PickFileToOpen(string commitText, params string[] filter)
    {
        var picker = CreateFileOpenPicker(commitText, filter);
        var result = await picker.PickSingleFileAsync();
        return result?.Path;
    }

    public async Task<IEnumerable<string>?> PickFilesToOpen(string commitText, params string[] filter)
    {
        var picker = CreateFileOpenPicker(commitText, filter);
        var results = await picker.PickMultipleFilesAsync();
        return results?.Select(f => f.Path);
    }

    public async Task<string?> PickPathToSave(string path, string commitText, params (string, IList<string>)[] fileTypeChoices)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);
        var picker = new FileSavePicker(MainWindow.Content.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            SuggestedFileName = fileName,
            DefaultFileExtension = extension,
            CommitButtonText = commitText,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        foreach(var (key, value) in fileTypeChoices)
            picker.FileTypeChoices.Add(key, value);
        var result = await picker.PickSaveFileAsync();
        return result?.Path;
    }

    FileOpenPicker CreateFileOpenPicker(string commitText, params string[] filter)
    {
        var picker = new FileOpenPicker(MainWindow.Content.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            CommitButtonText = commitText,
            ViewMode = PickerViewMode.Thumbnail,
        };
        foreach(var ext in filter)
            picker.FileTypeFilter.Add(ext);
        return picker;
    }

    public enum NavigationTarget
    {
        Back = -1,
        Main,
        Palette,
        ImagePalette,
        Settings
    }
}
