namespace ClientApp
{
    public partial class App : Application
    {
        public static bool IsAuthenticated { get; set; } = false;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
