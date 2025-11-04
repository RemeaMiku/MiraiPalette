using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
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
    public MainWindow MainWindow => Services.GetRequiredService<MainWindow>();

    public new static App Current => (App)Application.Current;

    public ServiceProvider Services { get; } = ConfigureServices();

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection()
            .AddSingleton<IPaletteDataService, MiraiPaletteDataFilePaletteDataService>()
            .AddTransient<PaletteDetailPageViewModel>()
            .AddTransient<ImagePalettePageViewModel>()
            .AddSingleton<MainPageViewModel>()
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
        MainWindow.Activate();
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
            PrimaryButtonText = "确认",
            SecondaryButtonText = "取消",
            IsSecondaryButtonEnabled = showCancelButton,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = MainWindow.Content.XamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<string?> PickFile(string commitText, params string[] filter)
    {
        var picker = CreateFileOpenPicker(commitText, filter);
        var result = await picker.PickSingleFileAsync();
        return result?.Path;
    }

    public async Task<IEnumerable<string>?> PickFiles(string commitText, params string[] filter)
    {
        var picker = CreateFileOpenPicker(commitText, filter);
        var results = await picker.PickMultipleFilesAsync();
        return results?.Select(f => f.Path);
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
