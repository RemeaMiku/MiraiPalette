namespace MiraiPalette.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var appShell = new AppShell();
            var window = new Window(appShell)
            {
                MinimumWidth = 350,
                MinimumHeight = 600,
            };
            return window;
        }
    }
}