
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
    void ToggleSidebar()
    {
        if(_isSideBarExpanded)
            new Animation(w => FlyoutWidth = w, DefaultFlyoutWidth, MinFlyoutWidth, Easing.Default).Commit(this, "FoldSideBar", length: 200);
        else
            new Animation(w => FlyoutWidth = w, MinFlyoutWidth, DefaultFlyoutWidth, Easing.Default).Commit(this, "ExpandSideBar", length: 200);
        _isSideBarExpanded = !_isSideBarExpanded;
    }
}
