using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels
{
    public partial class ActionViewModel : ObservableObject
    {
        [ObservableProperty]
        private int workerId = 1;

        [ObservableProperty]
        private int productId;

        [ObservableProperty]
        private string status = "Pending";

        [ObservableProperty]
        private string message = string.Empty;

        [RelayCommand]
        private async Task CreateActionAsync()
        {
            // Перевірка на від'ємне значення або нуль
            if (ProductId <= 0)
            {
                Message = "ID товару має бути додатним числом!";
                return;
            }

            Message = $"Заявку для товару #{ProductId} успішно створено!";
            await Task.Delay(1000);
            await Shell.Current.GoToAsync("//ItemsPage");
        }
    }
}
