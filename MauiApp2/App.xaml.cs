using MauiApp2.Data;

namespace MauiApp2
{
    public partial class App : Application
    {
        public static UserRepository UserRepository { get; private set; }
        
        public App(UserRepository userRepository)
        {
            InitializeComponent();

            MainPage = new AppShell();
            UserRepository = userRepository;
        }
    }
}