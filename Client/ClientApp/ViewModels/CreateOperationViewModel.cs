using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class CreateOperationViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public CreateOperationViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [RelayCommand]
        public async Task SaveOperationAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Резервна встановка BaseAddress під порт 5024
                if (_httpClient.BaseAddress == null)
                {
                    _httpClient.BaseAddress = new Uri("http://localhost:5024/");
                }

                // Вкажіть ваше DTO/модель для збереження операції
                var newAction = new 
                {
                    // Поля вашої моделі (наприклад, ItemId, Quantity, ActionType)
                };

                var response = await _httpClient.PostAsJsonAsync("api/actions", newAction);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успіх", "Операцію успішно збережено!", "OK");
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка", $"Код помилки сервера: {response.StatusCode}", "OK");
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