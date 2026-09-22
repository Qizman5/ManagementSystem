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

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
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
                    Items.Clear();

                    if (result != null && result.Count > 0)
                    {
                        foreach (var dto in result)
                        {
                            Items.Add(new Item
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
                        // Якщо сервер не дав даних, тихо підставляємо локальні без DisplayAlert
                        foreach (var item in FallbackItems)
                        {
                            Items.Add(item);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemsViewModel Fatal Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
