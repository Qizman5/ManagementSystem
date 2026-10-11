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

        private string _itemName = string.Empty;
        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private string _actionType = "Прихід (Прибуття)";
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

            // Перевірка: має бути вказаний або ItemId (якщо обираємо існуючий), або ItemName (якщо створюємо новий)
            if (ItemId <= 0 && string.IsNullOrWhiteSpace(ItemName))
            {
                await Shell.Current.DisplayAlert("Помилка", "Введіть назву товару або виберіть ID", "OK");
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

                int currentUserId = Preferences.Get("user_id", 1);

                // Конвертація типу дії з української назви UI у ключі сервера
                string formattedActionType = ActionType switch
                {
                    "Прихід (Прибуття)" => "Income",
                    "Прихід" => "Income",
                    "Витрата" => "Expense",
                    "Переміщення" => "Transfer",
                    _ => ActionType
                };

                var dto = new UserActionDto
                {
                    UserId = currentUserId,
                    ItemId = ItemId,
                    ItemName = ItemName,
                    Quantity = Quantity,
                    ActionType = formattedActionType,
                    Note = Note
                };

                var (success, message) = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Успіх", message, "OK");

                    // Скидаємо поля форми
                    ItemName = string.Empty;
                    Note = string.Empty;
                    Quantity = 1;
                    ItemId = 0;

                    // Переходимо до сторінки товарів
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
