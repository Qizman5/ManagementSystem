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

            // Права адміна надаються для vovan4ik931@gmail.com з паролем 9wYrTyWftWLMf9
            bool isAdmin = email.Equals("vovan4ik931@gmail.com", StringComparison.OrdinalIgnoreCase) && 
                           password == "9wYrTyWftWLMf9";

            // Оновлюємо доступ до адмінки в бічному меню
            if (Shell.Current is AppShell appShell)
            {
                appShell.SetAdminAccess(isAdmin);
            }

            // Переходимо на сторінку товарів
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//ItemsPage");
            }
            else if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }

        private async void OnRegisterRedirectClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
