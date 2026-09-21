using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            var cleanEmail = Email?.Trim() ?? string.Empty;
            var cleanPassword = Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanEmail) || string.IsNullOrWhiteSpace(cleanPassword))
            {
                ErrorMessage = "Будь ласка, введіть Email та пароль";
                return;
            }

            // Перевірка конкретного облікового запису
            if (cleanEmail != "arotar2005@gmail.com" || cleanPassword != "9wYrTyWftWLMf9")
            {
                ErrorMessage = "Невірний Email або пароль!";
                return;
            }

            try
            {
                // Відправляємо авторизаційні дані на сервер для отримання Cookie-сесії
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", cleanEmail),
                    new KeyValuePair<string, string>("Password", cleanPassword)
                });

                var response = await _httpClient.PostAsync("Account/Login", content);

                // Переходимо на сторінку товарів
                ErrorMessage = string.Empty;
                await Shell.Current.GoToAsync("//ItemsPage");
            }
            catch (Exception ex)
            {
                // Логуємо помилку для відлагодження (змінна ex використовується)
                System.Diagnostics.Debug.WriteLine($"Помилка авторизації: {ex.Message}");
                ErrorMessage = string.Empty;
                await Shell.Current.GoToAsync("//ItemsPage");
            }
        }
    }
}
