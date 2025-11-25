using CommunityToolkit.WinUI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PaletteDetailPage : Page
{
    public PaletteDetailPage()
    {
        InitializeComponent();
    }

    public PaletteDetailPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<PaletteDetailPageViewModel>();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var palette = e.Parameter as PaletteViewModel;
        ViewModel.LoadCommand.Execute(palette);
    }

    private void OnRootPageSizeChanged(object _, SizeChangedEventArgs e)
    {
        PaletteView.DisplayMode = e.NewSize.Width < PaletteView.OpenPaneLength * 2 ? SplitViewDisplayMode.Overlay : SplitViewDisplayMode.Inline;
        PaletteView.IsPaneOpen = ViewModel.IsEditingColor;
        //PaletteView.Focus(FocusState.Programmatic);
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
}
