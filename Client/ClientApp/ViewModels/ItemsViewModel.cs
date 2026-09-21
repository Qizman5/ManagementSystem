using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ItemsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await _httpClient.GetFromJsonAsync<List<Item>>("api/items");

                if (result != null)
                {
                    // Оновлення колекції робимо в UI-потоці без її перестворення (new)
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Items.Clear();
                        foreach (var item in result)
                        {
                            Items.Add(item);
                        }
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Помилка мережі: {ex.Message}";
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Помилка мережі", "Не вдалося з'єднатися з сервером.", "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Виникла помилка: {ex.Message}";
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
