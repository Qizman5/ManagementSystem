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

        private string _username = "arotar2005@gmail.com";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

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

                // Перевіряємо, чи це обліковий запис адміністратора
                bool isAdmin = cleanUsername.Equals("arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase);

                // 1. Спроба авторизації через backend API
                bool isSuccess = await _apiService.LoginAsync(cleanUsername, cleanPassword);

                // Якщо сервер повернув true АБО це логін адміна — пропускаємо в систему
                if (isSuccess || isAdmin)
                {
                    // 2. Гарантовано записуємо токен у сховище
                    await SecureStorage.Default.SetAsync("jwt_token", "admin_authenticated_session_token");

                    // 3. Активуємо адмін-панель у Flyout-меню
                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.SetAdminAccess(isAdmin);
                    }

                    // 4. Невеликий затримка для гарантованого збереження сховища перед навігацією
                    await Task.Delay(100);

                    // 5. Переходимо на головну сторінку
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
                
                // Резервний вход для адміна у випадку відсутності зв'язку з сервером
                if (cleanUsername.Equals("arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    await SecureStorage.Default.SetAsync("jwt_token", "admin_offline_token");
                    if (Shell.Current is AppShell appShell) appShell.SetAdminAccess(true);
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка мережі", "Не вдалося з'єднатися з сервером", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
