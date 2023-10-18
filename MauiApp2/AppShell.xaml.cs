namespace MauiApp2
{
    public partial class AppShell : Shell
    {
        bool blue;
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
            Routing.RegisterRoute(nameof(VideoPage),typeof(VideoPage));
            Routing.RegisterRoute(nameof(btPage),typeof(btPage));
            //Routing.RegisterRoute(nameof(test), typeof(test));
        }

        public void SetBluetooth(bool set)
        {
            blue = set;
        }

        public bool GetBluetooth()
        {
            return blue;
        }
    }
}