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

        // Встановлюємо email за замовчуванням
        private string _username = "arotar2005@gmail.com";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        // Встановлюємо пароль за замовчуванням
        private string _password = "9wYrTyWftWLMf9";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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

                // 1. Виконуємо запит авторизації до API
                bool isSuccess = await _apiService.LoginAsync(cleanUsername, cleanPassword);

                if (isSuccess)
                {
                    // 2. Зберігаємо токен авторизації перед переходом
                    await SecureStorage.Default.SetAsync("jwt_token", "authenticated_user_session");

                    // 3. Перевіряємо, чи є користувач адміністратором
                    bool isAdmin = cleanUsername.Equals("arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase);

                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.SetAdminAccess(isAdmin);
                    }

                    // 4. Тільки після повного збереження токена виконуємо перехід
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
