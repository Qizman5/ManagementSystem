namespace ClientApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Додаткова реєстрація маршруту для модальної або програмної навігації
            Routing.RegisterRoute(nameof(Views.CreateOperationPage), typeof(Views.CreateOperationPage));
        }
    }
}
