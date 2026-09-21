using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ClientApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            _apiService = new ApiService();
            LoginCommand = new Command(async () => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            if (IsBusy) return;

            var cleanUsername = Username?.Trim() ?? string.Empty;
            var cleanPassword = Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(cleanPassword))
            {
                await Shell.Current.DisplayAlert("Помилка", "Будь ласка, введіть логін та пароль", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // 1. Запит до API (POST /api/auth/login)
                string? token = await _apiService.LoginAsync(cleanUsername, cleanPassword);

                if (!string.IsNullOrEmpty(token))
                {
                    // 2. Збереження JWT-токена у Preferences
                    Preferences.Set("jwt_token", token);

                    // 3. Успішний перехід до сторінки товарів
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка входу", "Невірний логін або пароль", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoginViewModel Exception]: {ex.Message}");
                await Shell.Current.DisplayAlert("Помилка мережі", "Не вдалося з'єднатися з сервером", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
