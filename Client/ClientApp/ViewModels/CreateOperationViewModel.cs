using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using ClientApp.Services;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class CreateOperationViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        // Колекція використовує ItemDto, який повертає ApiService
        public ObservableCollection<ItemDto> Items { get; } = new();

        private ItemDto? _selectedItem;
        public ItemDto? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (value != null)
                    {
                        ItemId = value.Id;
                    }
                }
            }
        }

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
            _ = LoadItemsAsync();
        }

        public async Task LoadItemsAsync()
        {
            try
            {
                var fetchedItems = await _apiService.GetItemsAsync();
                Items.Clear();
                if (fetchedItems != null)
                {
                    foreach (var item in fetchedItems)
                    {
                        Items.Add(item); // Тепер типи збігаються (ItemDto -> ItemDto)
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CreateOperationVM LoadItems Error]: {ex.Message}");
            }
        }

        private async Task SaveOperationAsync()
        {
            if (IsBusy) return;

            if (SelectedItem == null && ItemId <= 0)
            {
                await Shell.Current.DisplayAlert("Увага", "Будь ласка, оберіть товар зі списку!", "OK");
                return;
            }

            if (Quantity <= 0)
            {
                await Shell.Current.DisplayAlert("Увага", "Кількість має бути більшою за 0", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var dto = new UserActionDto
                {
                    UserId = 3,
                    ItemId = SelectedItem?.Id ?? ItemId,
                    Quantity = Quantity,
                    ActionType = ActionType,
                    Note = Note
                };

                bool success = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Успіх", "Операцію успішно збережено!", "OK");
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка", "Не вдалося виконати операцію на сервері.", "OK");
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
