namespace MauiApp2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
            Routing.RegisterRoute(nameof(VideoPage),typeof(VideoPage));
            Routing.RegisterRoute(nameof(btPage),typeof(btPage));
            //Routing.RegisterRoute(nameof(test), typeof(test));
            Routing.RegisterRoute(nameof(graphtest), typeof(graphtest));
        }
        
    }
}