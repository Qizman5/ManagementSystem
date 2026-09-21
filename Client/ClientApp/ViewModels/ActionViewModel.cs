using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Services;
using Microsoft.Maui.Controls;

namespace ClientApp.ViewModels
{
    public partial class ActionViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<ItemDto> _items = new();

        [ObservableProperty]
        private ItemDto? _selectedItem;

        [ObservableProperty]
        private string _selectedActionType = "Прихід";

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private bool _isBusy;

        public ObservableCollection<string> ActionTypes { get; } = new()
        {
            "Прихід",
            "Витрата",
            "Переміщення"
        };

        public ActionViewModel()
        {
            _apiService = new ApiService();
            _ = LoadItemsAsync();
        }

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            var result = await _apiService.GetItemsAsync();
            if (result != null)
            {
                Items.Clear();
                foreach (var item in result)
                {
                    Items.Add(item);
                }
            }
        }

        [RelayCommand]
        public async Task CreateActionAsync()
        {
            if (IsBusy) return;

            if (SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("Помилка", "Будь ласка, виберіть товар зі списку", "OK");
                return;
            }

            if (Quantity <= 0)
            {
                await Shell.Current.DisplayAlert("Помилка", "Кількість повинна бути більше 0", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var dto = new UserActionDto
                {
                    ItemId = SelectedItem.Id,
                    ActionType = SelectedActionType,
                    Quantity = Quantity
                };

                bool success = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Успіх", "Операцію зафіксовано на сервері!", "OK");
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка", "Не вдалося зберегти операцію на сервері", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ActionViewModel Exception]: {ex.Message}");
                await Shell.Current.DisplayAlert("Помилка", "Сталася помилка з'єднання", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
