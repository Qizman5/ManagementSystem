using Microsoft.Maui.Controls;

namespace ClientApp.Views
{
    public partial class ProductsPage : ContentPage
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Очищаємо збережений токен авторизації
            Preferences.Remove("AuthToken");
            
            // Перенаправляємо на сторінку входу
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
