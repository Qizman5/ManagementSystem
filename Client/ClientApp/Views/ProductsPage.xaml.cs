using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ClientApp.Views
{
    public partial class ProductsPage : ContentPage
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Очищаємо токен авторизації
            Preferences.Remove("AuthToken");

            // Безпечна зміна головної сторінки з перевіркою на null
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
