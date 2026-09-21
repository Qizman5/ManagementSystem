using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Services;
using Microsoft.Maui.ApplicationModel;

namespace ClientApp.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public static List<ItemDto> SharedItems { get; } = new()
        {
            new ItemDto { Id = 1, Name = "Палета дерев'яна", Quantity = 50 },
            new ItemDto { Id = 2, Name = "Коробка картонна (L)", Quantity = 120 },
            new ItemDto { Id = 3, Name = "Стретч-плівка 20мкм", Quantity = 15 }
        };

        [ObservableProperty]
        private ObservableCollection<ItemDto> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ItemsViewModel()
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
                ErrorMessage = string.Empty;

                var result = await _apiService.GetItemsAsync();

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

                LoadFallbackItems();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemsViewModel Error]: {ex.Message}");
                LoadFallbackItems();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadFallbackItems()
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
