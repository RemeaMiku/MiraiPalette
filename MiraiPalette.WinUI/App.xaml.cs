using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using MiraiPalette.Shared.Data;
using MiraiPalette.Shared.Data.Impl;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.Services.Impl;
using MiraiPalette.WinUI.Settings;
using MiraiPalette.WinUI.ViewModels;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public MainWindow MainWindow => Services.GetRequiredService<MainWindow>();

    public ISettingsService SettingsService => Services.GetRequiredService<ISettingsService>();

    public new static App Current => (App)Application.Current;

    public ServiceProvider Services { get; } = ConfigureServices();

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection()
            .AddDbContext<MiraiPaletteDb, LocalMiraiPaletteDb>(options => options.UseSqlite($"Data Source={Path.Combine(MiraiPaletteDbStorageService.DbFolderPath, MiraiPaletteDbStorageService.DbName)}"))
            .AddScoped<IMiraiPaletteStorageService, MiraiPaletteDbStorageService>()
            .AddSingleton<IPaletteFileService, PaletteFileService>()
            .AddSingleton<ISettingsService, LocalSettingsService>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddTransient<PaletteDetailPageViewModel>()
            .AddTransient<ImagePalettePageViewModel>()
            .AddTransient<MainPageViewModel>()
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
        ExitIfAnotherInstanceRunning();
        base.OnLaunched(args);
        ApplySettings();
        MainWindow.Activate();
    }

    #region Settings

    public void ApplySettings()
    {
        ApplyThemeModeSetting();
        ApplyNavigationStyleSetting();
        ApplyLanguageSetting();
    }

    private void HandleTitleBarThemeChange(ElementTheme theme)
    {
        var titleBar = MainWindow.AppWindow.TitleBar;
        var foregroundColor = theme == ElementTheme.Dark ? Colors.White : Colors.Black;
        titleBar.ButtonForegroundColor = foregroundColor;
        titleBar.ButtonHoverForegroundColor = foregroundColor;
        var backgroundHoverColor = theme == ElementTheme.Dark ? Color.FromArgb(24, 255, 255, 255) : Color.FromArgb(24, 0, 0, 0);
        titleBar.ButtonHoverBackgroundColor = backgroundHoverColor;
    }

    public void ApplyThemeModeSetting()
    {
        var setting = SettingsService.GetValue(SettingKeys.Theme, ElementTheme.Default);
        var frameworkElement = MainWindow.Content as FrameworkElement;
        frameworkElement!.RequestedTheme = setting;
        // Apply title bar theme manually
        HandleTitleBarThemeChange(frameworkElement.ActualTheme);
    }

    public void ApplyNavigationStyleSetting()
    {
        var setting = SettingsService.GetValue(SettingKeys.NavigationStyle, NavigationViewPaneDisplayMode.Auto);
        // only Left and Top are supported
        if(setting is not NavigationViewPaneDisplayMode.Top and not NavigationViewPaneDisplayMode.Auto)
        {
            setting = NavigationViewPaneDisplayMode.Auto;
            SettingsService.SetValue(SettingKeys.NavigationStyle, setting);
        }
        MainWindow.MainView.PaneDisplayMode = setting;
    }

    public void ApplyLanguageSetting()
    {
        var languageSetting = SettingsService.GetValue(SettingKeys.Language, LanguageSettings.Default);
        var isSupported = LanguageSettings.TryConvertSettingToActual(languageSetting, out var language);
        // If the setting is unsupported, we update it to the actual language
        if(!isSupported)
            SettingsService.SetValue(SettingKeys.Language, language);
        var culture = new CultureInfo(language);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    #endregion

    #region Dialogs
    public async Task<bool> ShowConfirmDialogAsync(string title, string content, bool showCancelButton = true, string? primaryButtonText = null)
    {
        primaryButtonText ??= Strings.Resources.Confirm;
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = showCancelButton ? Strings.Resources.Cancel : default,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = MainWindow.Content.XamlRoot,
            RequestedTheme = (MainWindow.Content as FrameworkElement)!.RequestedTheme
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<bool> ShowDeleteConfirmDialogAsync(string title, string content)
        => await ShowConfirmDialogAsync(title, content, true, Strings.Resources.Delete);

    #endregion

    #region Pickers
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

    public async Task<string?> PickFolder(string commitText)
    {
        var picker = new FolderPicker(MainWindow.Content.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            CommitButtonText = commitText,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        var result = await picker.PickSingleFolderAsync();
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
    #endregion

    #region Single Instance

    private static Mutex? _mutex;
    private const string _mutexName = "MiraiPalette_SingleInstance";

    private static void ExitIfAnotherInstanceRunning()
    {
        _mutex = new Mutex(true, _mutexName, out var createdNew);
        if(!createdNew)
        {
            ActivateExistingInstance();
            Environment.Exit(0);
        }
    }

    private static void ActivateExistingInstance()
    {
        var current = Process.GetCurrentProcess();

        var existing = Process.GetProcessesByName(current.ProcessName)
                              .FirstOrDefault(p => p.Id != current.Id);

        if(existing == null)
            return;

        ActivateMainWindow(existing.Id);
    }


    private static void ActivateMainWindow(int processId)
    {
        EnumWindows((hWnd, lParam) =>
        {
            GetWindowThreadProcessId(hWnd, out int pid);
            if(pid != processId)
                return true;

            if(!IsWindowVisible(hWnd))
                return true;

            ShowWindow(hWnd, SW_RESTORE);
            SetForegroundWindow(hWnd);
            return false; // 找到就停
        }, IntPtr.Zero);
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    private static partial uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    private const int SW_RESTORE = 9;

    #endregion
}
