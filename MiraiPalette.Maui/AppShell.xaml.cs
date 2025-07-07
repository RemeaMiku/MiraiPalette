
namespace MiraiPalette.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            OnSideBarButtonClicked(default!, default!);
        }

        private bool _isSideBarExpanded = false;

        private void OnSideBarButtonClicked(object _, EventArgs e)
        {
            if(_isSideBarExpanded)            
               new Animation(w => FlyoutWidth = w,250,55,Easing.Default).Commit(this, "FoldSideBar", length:200);            
            else            
                new Animation(w => FlyoutWidth = w, 55, 250, Easing.Default).Commit(this, "ExpandSideBar", length: 200);            
            _isSideBarExpanded = !_isSideBarExpanded;
        }
    }
}
