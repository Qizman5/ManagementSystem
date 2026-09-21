using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        // Спільний локальний список для збереження товарів під час сесії
        public static List<Item> SharedItems { get; } = new List<Item>
        {
            new Item { Id = 1, Name = "Палета дерев'яна", OperationType = "Прихід (Прибуття)", Quantity = 50, Price = 150 },
            new Item { Id = 2, Name = "Коробка картонна (L)", OperationType = "Витрата (Відвантаження)", Quantity = 120, Price = 25 },
            new Item { Id = 3, Name = "Стретч-плівка 20мкм", OperationType = "Переміщення", Quantity = 15, Price = 210 }
        };

        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ItemsViewModel() : this(new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5024/") })
        {
        }

        public ItemsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadFallbackItems();
        }

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (_httpClient.BaseAddress == null)
                {
                    _httpClient.BaseAddress = new Uri("http://127.0.0.1:5024/");
                }

                var response = await _httpClient.GetAsync("items");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<Item>>();

                    if (result != null && result.Count > 0)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Items.Clear();
                            foreach (var item in result)
                            {
                                Items.Add(item);
                            }
                        });
                        return;
                    }
                }

                LoadFallbackItems();
            }
            catch
            {
                // Повертаємо локальні товари (включаючи ті, що ви додали через форму)
                LoadFallbackItems();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void LoadFallbackItems()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Items.Clear();
                foreach (var item in SharedItems)
                {
                    Items.Add(item);
                }
            });
        }
    }
}
