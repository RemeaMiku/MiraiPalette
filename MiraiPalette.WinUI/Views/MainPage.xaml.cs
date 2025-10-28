using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.LoadCommand.Execute(null);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

    }

    public MainPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<MainPageViewModel>();

    private async void OnCopyColorButton_Click(object sender, RoutedEventArgs e)
    {
        if(sender is Button button)
        {
            // 切换内容
            var grid = button.Content as Grid;
            if(grid != null)
            {
                var normal = grid.FindName("CopyNormalContent") as StackPanel;
                var copied = grid.FindName("CopyCopiedContent") as StackPanel;
                if(normal != null && copied != null)
                {
                    normal.Visibility = Visibility.Collapsed;
                    copied.Visibility = Visibility.Visible;
                }
            }

            // 等待1.5秒
            await Task.Delay(1500);

            VisualStateManager.GoToState(button, "Normal", true);
            if(grid != null)
            {
                var normal = grid.FindName("CopyNormalContent") as StackPanel;
                var copied = grid.FindName("CopyCopiedContent") as StackPanel;
                if(normal != null && copied != null)
                {
                    normal.Visibility = Visibility.Visible;
                    copied.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    private void OnPageSizeChanged(object _, SizeChangedEventArgs e)
    {
        MainView.DisplayMode = e.NewSize.Width < MainView.OpenPaneLength * 2 ? SplitViewDisplayMode.Overlay : SplitViewDisplayMode.Inline;
        MainView.Focus(FocusState.Programmatic);
    }

    private async void OnExtractFromImageButtonClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            CommitButtonText = "选择",
            FileTypeFilter = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" },
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            ViewMode = PickerViewMode.Thumbnail
        };
        var result = await picker.PickSingleFileAsync();
        if(result is not null)
            Current.NavigateTo(NavigationTarget.ImagePalette, result.Path);
    }
}
