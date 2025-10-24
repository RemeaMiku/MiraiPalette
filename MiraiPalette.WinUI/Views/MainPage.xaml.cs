using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
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
        _editColorFlyout = Resources["MiraiColorEditColorFlyout"] as Flyout ?? throw new InvalidOperationException("无法找到 MiraiColorEditColorFlyout");
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.LoadCommand.Execute(null);
    }

    readonly Flyout _editColorFlyout;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(ViewModel.CurrentPalette):
                ColorsScrollViewer.ScrollToVerticalOffset(0);
                break;
            default:
                break;
        }
    }

    public MainPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<MainPageViewModel>();

    private void OnSaveColorButton_Click(object sender, RoutedEventArgs e)
    {
        _editColorFlyout.Hide();
    }

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

    #region ColorPicker 初始值修复
    // 社区工具包的 ColorPicker 第一次绑定 Color 属性时会莫名其妙地把 Color 设置为透明色 #00FFFFFF
    // 这是一个已知问题，值会按以下方式改变：#FFFFFFFF => #00000000 => 绑定源 => #00FFFFFF
    // https://github.com/CommunityToolkit/Windows/blob/main/components/ColorPicker/src/ColorPicker.cs

    int _colorPickerFallbackCount = 0;
    private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if(args.NewColor.ToHex() == "#00FFFFFF")
        {
            // 恢复到旧值
            sender.Color = args.OldColor;
            _colorPickerFallbackCount++;
            // 自动回弹会出现2次
            if(_colorPickerFallbackCount == 2)
                sender.ColorChanged -= ColorPicker_ColorChanged;
        }
    }
    #endregion

    private async void OnAddColorButton_Click(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        ColorsScrollViewer.ScrollToVerticalOffset(ColorsScrollViewer.ScrollableHeight);
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
