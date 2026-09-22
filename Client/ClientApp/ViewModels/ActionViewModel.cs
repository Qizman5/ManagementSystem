using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Models;
using ClientApp.Services;

namespace ClientApp.ViewModels
{
    public partial class ActionViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private Item? _selectedItem;

        [ObservableProperty]
        private string _actionType = "Прихід (Прибуття)";

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private string _note = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ActionViewModel()
        {
            _apiService = new ApiService();
            _ = LoadItemsAsync();
        }

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Items.Clear();

                var dtos = await _apiService.GetItemsAsync();
                if (dtos != null && dtos.Count > 0)
                {
                    foreach (var dto in dtos)
                    {
                        Items.Add(new Item { Id = dto.Id, Name = dto.Name, Quantity = dto.Quantity });
                    }
                }
                else
                {
                    foreach (var fallback in ItemsViewModel.FallbackItems)
                    {
                        Items.Add(fallback);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ActionViewModel Load Error]: {ex.Message}");
                foreach (var fallback in ItemsViewModel.FallbackItems)
                {
                    Items.Add(fallback);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CreateActionAsync()
        {
            if (SelectedItem == null) return;

            try
            {
                IsBusy = true;
                // Імітація або відправка запиту до API
                await Task.Delay(500); 
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
