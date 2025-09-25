using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MiraiPalette.WinUI.ViewModels;
using MiraiPalette.WinUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        MainFrame.Content = App.Current.Services.GetRequiredService<MainPage>();
        var presenter = AppWindow.Presenter as OverlappedPresenter;
        presenter!.PreferredMinimumWidth = 480;
        presenter.PreferredMinimumHeight = 640;
    }

    public MainWindowViewModel ViewModel { get; } = App.Current.Services.GetRequiredService<MainWindowViewModel>();
}
