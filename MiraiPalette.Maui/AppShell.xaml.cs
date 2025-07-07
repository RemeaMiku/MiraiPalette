
namespace MiraiPalette.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private bool _isSideBarExpanded = true;

        private void OnSideBarButtonClicked(object sender, EventArgs e)
        {
            if(_isSideBarExpanded)            
               new Animation(w => FlyoutWidth = w,200,55,Easing.Default).Commit(this, "FoldSideBar", length:200);            
            else            
                new Animation(w => FlyoutWidth = w, 55, 200, Easing.Default).Commit(this, "ExpandSideBar", length: 200);            
            _isSideBarExpanded = !_isSideBarExpanded;
        }
    }
}
