using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.Pages.Controls;

public partial class ColorView : ContentView
{
    public ColorView()
    {
        InitializeComponent();
        ToolTipProperties.SetText(_hexButton, StringResource.CopyToClipboard);
        _copiedLabel.Text = StringResource.Copied;
    }

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        _copiedLabel.IsVisible = true;
        _copiedLabel.Opacity = 1;
        var button = (Button)sender;
        var text = button.Text;
        await Clipboard.SetTextAsync(text);
        await _copiedLabel.FadeTo(0, 1500, Easing.CubicIn);
        _copiedLabel.IsVisible = false;
    }
}