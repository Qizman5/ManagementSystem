using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ItemsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [RelayCommand]
        public async Task LoadItemsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Item>>("api/items");
                if (result != null)
                {
                    Items = new ObservableCollection<Item>(result);
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Помилка завантаження даних: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}