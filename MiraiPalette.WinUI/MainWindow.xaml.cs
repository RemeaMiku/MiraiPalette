using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{

    public const int MinWindowWidth = 480;

    public const int MinWindowHeight = 640;

    public const int OverlayThresholdWidth = 800;

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        var presenter = AppWindow.Presenter as OverlappedPresenter;
        presenter!.PreferredMinimumWidth = MinWindowWidth;
        presenter.PreferredMinimumHeight = MinWindowHeight;
    }

    public MainWindowViewModel ViewModel { get; } = App.Current.Services.GetRequiredService<MainWindowViewModel>();

    public Frame NavigationFrame => ContentFrame;

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        App.Current.NavigateTo(NavigationTarget.Back);
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        MainNavigationView.IsPaneOpen = !MainNavigationView.IsPaneOpen;
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        TitleBar.IsPaneToggleButtonVisible = Window.Bounds.Width < OverlayThresholdWidth;
    }

    private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if(args.IsSettingsSelected)
        {
            App.Current.NavigateTo(NavigationTarget.Settings);
            return;
        }
        var selectedItem = args.SelectedItem as NavigationViewItem;
        switch(selectedItem?.Tag)
        {
            // 临时
            case null:
                App.Current.NavigateTo(NavigationTarget.Main);
                break;
        }
    }
}
