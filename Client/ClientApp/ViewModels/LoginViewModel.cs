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
            // Видаляємо зайві пробіли з початку і кінця
            var cleanEmail = Email?.Trim() ?? string.Empty;
            var cleanPassword = Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanEmail) || string.IsNullOrWhiteSpace(cleanPassword))
            {
                ErrorMessage = "Будь ласка, заповніть усі поля!";
                return;
            }

            ErrorMessage = string.Empty;
            
            // Прямий перехід на сторінку товарів
            await Shell.Current.GoToAsync("//ItemsPage");
        }
    }
}