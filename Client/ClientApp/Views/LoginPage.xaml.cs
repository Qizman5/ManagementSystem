using System;
using Microsoft.Maui.Controls;

namespace ClientApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? string.Empty;
            string password = PasswordEntry.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Помилка", "Будь ласка, введіть email та пароль", "OK");
                return;
            }

            // Перехід на головну сторінку списку товарів у межах NavigationPage
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new ItemsPage());
            }
        }

        private async void OnRegisterRedirectClicked(object sender, EventArgs e)
        {
            // Перехід на сторінку реєстрації
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
