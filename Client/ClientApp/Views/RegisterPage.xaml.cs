using System;
using Microsoft.Maui.Controls;

namespace ClientApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string email = RegEmailEntry.Text?.Trim() ?? string.Empty;
            string password = RegPasswordEntry.Text ?? string.Empty;
            string confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Помилка", "Будь ласка, заповніть усі поля", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Помилка", "Паролі не збігаються", "OK");
                return;
            }

            await DisplayAlert("Успіх", "Акаунт успішно створено!", "OK");

            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new ItemsPage());
            }
        }

        private async void OnLoginRedirectClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
