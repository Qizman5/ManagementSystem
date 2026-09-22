using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Models;
using ClientApp.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace ClientApp.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        // Приватний кеш та прапорець ініціалізації для оптимізації UI
        private List<Item> _cachedItems = new();
        private bool _isInitialized = false;

        public static List<Item> FallbackItems { get; } = new()
        {
            new Item { Id = 1, Name = "Палета дерев'яна", Quantity = 50, OperationType = "Прихід (Прибуття)" },
            new Item { Id = 2, Name = "Коробка картонна (L)", Quantity = 120, OperationType = "Витрата (Відвантаження)" },
            new Item { Id = 3, Name = "Стретч-плівка 20мкм", Quantity = 15, OperationType = "Переміщення" }
        };

        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ItemsViewModel()
        {
            _apiService = new ApiService();
            _ = LoadItemsAsync();
        }

        // Перехід на сторінку створення операції
        [RelayCommand]
        private async Task GoToCreateOperationAsync()
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//CreateOperationPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Navigation Error]: {ex.Message}");
            }
        }

        // Оптимізований метод завантаження товарів із підтримкою кешу та параметра примусового оновлення
        [RelayCommand]
        public async Task LoadItemsAsync(bool forceRefresh = false)
        {
            // Якщо дані вже завантажені і не вимагається примусове оновлення — беремо з кешу
            if (_isInitialized && !forceRefresh && _cachedItems.Count > 0)
            {
                UpdateItemsCollection(_cachedItems);
                return;
            }

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                IsRefreshing = true;
                ErrorMessage = string.Empty;

                List<ItemDto>? result = null;

                try
                {
                    result = await _apiService.GetItemsAsync();
                }
                catch (Exception netEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[Network Error]: {netEx.Message}");
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var newList = new List<Item>();

                    if (result != null && result.Count > 0)
                    {
                        foreach (var dto in result)
                        {
                            newList.Add(new Item
                            {
                                Id = dto.Id,
                                Name = dto.Name,
                                Quantity = dto.Quantity,
                                OperationType = "Прихід (Прибуття)"
                            });
                        }
                    }
                    else
                    {
                        // Якщо сервер не дав даних, підставляємо локальні резервні дані
                        foreach (var item in FallbackItems)
                        {
                            newList.Add(item);
                        }
                    }

                    // Зберігаємо в кеш та оновлюємо колекцію екрана
                    _cachedItems = newList;
                    _isInitialized = true;
                    UpdateItemsCollection(_cachedItems);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemsViewModel Fatal Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private void UpdateItemsCollection(List<Item> sourceList)
        {
            Items.Clear();
            foreach (var item in sourceList)
            {
                Items.Add(item);
            }
        }
    }
}
