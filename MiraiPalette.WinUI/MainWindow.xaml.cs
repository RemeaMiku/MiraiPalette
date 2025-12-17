using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MiraiPalette.WinUI.Essentials.Navigation;
using MiraiPalette.WinUI.Helpers;
using MiraiPalette.WinUI.ViewModels;
using MiraiPalette.WinUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window, IRecipient<NavigationMessage>
{

    public const int MinWindowWidth = 520;

    public const int MinWindowHeight = 640;

    public const int OverlayThresholdWidth = 800;

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        var presenter = AppWindow.Presenter as OverlappedPresenter;
        presenter!.PreferredMinimumWidth = MinWindowWidth;
        presenter.PreferredMinimumHeight = MinWindowHeight;
        Messenger.RegisterAll(this);
        ViewModel.NewFolderAdded += ViewModel_NewFolderAdded;
        Activated += MainWindow_Activated;
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        ViewModel.IsActive = true;
    }

    private async void ViewModel_NewFolderAdded(object? sender, FolderViewModel e)
    {
        await Task.Delay(100);
        ScrollHelper.ScrollIntoView(MainNavigationView, e);
    }

    public IMessenger Messenger { get; } = App.Current.Services.GetRequiredService<IMessenger>();

    public MainWindowViewModel ViewModel { get; } = App.Current.Services.GetRequiredService<MainWindowViewModel>();

    public TitleBar TitleBarControl => TitleBar;

    public NavigationView MainView => MainNavigationView;

    public Frame NavigationFrame => ContentFrame;

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        Navigate(NavigationTarget.Back);
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        MainNavigationView.IsPaneOpen = !MainNavigationView.IsPaneOpen;
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        TitleBar.IsPaneToggleButtonVisible = MainNavigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top && Window.Bounds.Width < OverlayThresholdWidth;
    }

    private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        //if(args.IsSettingsSelected)
        //{
        //    App.Current.NavigateTo(NavigationTarget.Settings);
        //    return;
        //}
        //App.Current.NavigateTo(NavigationTarget.Main);
        //var selectedItem = args.SelectedItem as NavigationViewItem;
        //switch(selectedItem?.Tag)
        //{
        //    // 临时
        //    case null:
        //        App.Current.NavigateTo(NavigationTarget.Main);
        //        break;
        //}
    }

    private void MainNavigationView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        MenuSeparator.Visibility = sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top ? Visibility.Collapsed : Visibility.Visible;
    }

    private void Navigate(NavigationTarget target, object? parameter = null)
    {
        switch(target)
        {
            case NavigationTarget.Main:
                ContentFrame.Navigate(typeof(MainPage));
                ContentFrame.BackStack.Clear();
                break;
            case NavigationTarget.Palette:
                ContentFrame.Navigate(typeof(PaletteDetailPage));
                break;
            case NavigationTarget.ImagePalette:
                ContentFrame.Navigate(typeof(ImagePalettePage));
                break;
            case NavigationTarget.Settings:
                ContentFrame.Navigate(typeof(SettingsPage));
                ContentFrame.BackStack.Clear();
                break;
            case NavigationTarget.Back:
                if(ContentFrame.CanGoBack)
                    ContentFrame.GoBack();
                break;
            default:
                break;
        }
        var currentPage = ContentFrame.Content as Page;
        var currentViewModel = currentPage?.GetType().GetProperty("ViewModel")?.GetValue(currentPage) as PageViewModelBase;
        currentViewModel?.OnNavigatedTo(parameter);
    }

    public void Receive(NavigationMessage message)
    {
        Navigate(message.Target, message.Parameter);
    }
}
