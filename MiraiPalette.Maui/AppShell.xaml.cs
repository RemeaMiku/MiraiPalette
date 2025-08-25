using CommunityToolkit.Mvvm.Input;

namespace MiraiPalette.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private bool _isSideBarExpanded = true;

    public const double DefaultFlyoutWidth = 250;
    public const double MinFlyoutWidth = 65;

    private void OnSideBarButtonClicked(object _, EventArgs e)
    {
        FlyoutIsPresented = !FlyoutIsPresented;
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        FlyoutIsPresented = !FlyoutIsPresented;
    }
}