namespace MiraiPalette.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public const double DefaultFlyoutWidth = 250;
}