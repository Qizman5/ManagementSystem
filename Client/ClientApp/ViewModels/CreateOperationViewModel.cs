using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ClientApp.Services;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class CreateOperationViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private int _itemId;
        public int ItemId
        {
            get => _itemId;
            set
            {
                if (SetProperty(ref _itemId, value))
                {
                    OnPropertyChanged(nameof(ProductId));
                }
            }
        }

        public int ProductId
        {
            get => ItemId;
            set => ItemId = value;
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private string _actionType = "Income";
        public string ActionType
        {
            get => _actionType;
            set => SetProperty(ref _actionType, value);
        }

        private string _note = string.Empty;
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public string Message
        {
            get => ErrorMessage;
            set => ErrorMessage = value;
        }

        public IAsyncRelayCommand SaveOperationCommand { get; }
        public IAsyncRelayCommand CreateActionCommand => SaveOperationCommand;

        public CreateOperationViewModel()
        {
            _apiService = new ApiService();
            SaveOperationCommand = new AsyncRelayCommand(SaveOperationAsync);
        }

        private async Task SaveOperationAsync()
        {
            if (IsBusy) return;

            if (ItemId <= 0)
            {
                await Shell.Current.DisplayAlert("Помилка", "Введіть коректний ID товару", "OK");
                return;
            }

            if (Quantity <= 0)
            {
                await Shell.Current.DisplayAlert("Помилка", "Кількість має бути більшою за 0", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Отримуємо ID збереженого користувача або використовуємо 1 за замовчуванням
                int currentUserId = Preferences.Get("user_id", 1);

                var dto = new UserActionDto
                {
                    UserId = currentUserId,
                    ItemId = ItemId,
                    Quantity = Quantity,
                    ActionType = ActionType,
                    Note = Note
                };

                var (success, message) = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Успіх", message, "OK");
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    ErrorMessage = message;
                    await Shell.Current.DisplayAlert("Помилка", message, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Виникла помилка: {ex.Message}";
                await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
