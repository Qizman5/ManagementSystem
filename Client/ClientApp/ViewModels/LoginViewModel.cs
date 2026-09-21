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
            var cleanEmail = Email?.Trim() ?? string.Empty;
            var cleanPassword = Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanEmail) || string.IsNullOrWhiteSpace(cleanPassword))
            {
                ErrorMessage = "Будь ласка, введіть Email та пароль";
                return;
            }

            if (cleanEmail != "arotar2005@gmail.com" || cleanPassword != "9wYrTyWftWLMf9")
            {
                ErrorMessage = "Невірний Email або пароль!";
                return;
            }

            ErrorMessage = string.Empty;
            await Shell.Current.GoToAsync("//ItemsPage");
        }
    }
}
