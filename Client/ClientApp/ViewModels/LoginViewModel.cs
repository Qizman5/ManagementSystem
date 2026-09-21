using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            // Перевірка порожніх полів
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Будь ласка, введіть Email та пароль";
                return;
            }

            // Перевірка конкретних даних входу
            if (Email != "arotar2005@gmail.com" || Password != "9wYrTyWftWLMf9")
            {
                ErrorMessage = "Невірний Email або пароль!";
                return;
            }

            ErrorMessage = string.Empty;
            await Shell.Current.GoToAsync("//ItemsPage");
        }
    }
}
