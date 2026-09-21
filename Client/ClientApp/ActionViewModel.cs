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
            if (ProductId <= 0)
            {
                Message = "������ ��������� ID ������!";
                return;
            }

            Message = $"�������� ��� ������ #{ProductId} ������ ��������!";
            await Task.Delay(1500);
            await Shell.Current.GoToAsync("//ItemsPage");
        }
    }
}
