namespace MiraiPalette.Maui.Pages.Controls;

public partial class ColorView : ContentView
{
    public ColorView()
    {
        InitializeComponent();
    }

    private void CopyButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var text = button.Text;
        Clipboard.SetTextAsync(text);
    }
}