using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClientApp.Models;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace ClientApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Динамічний вибір IP залежно від платформи (Android Emulator vs Windows/iOS)
        public static string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5024/"
            : "http://localhost:5024/";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        /// <summary>
        /// Додає JWT-токен у заголовок Authorization для захищених запитів
        /// </summary>
        private async Task AddAuthHeaderAsync()
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token") 
                        ?? Preferences.Get("jwt_token", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// Авторизація користувача та збереження JWT-токена
        /// </summary>
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", result.Token);
                        Preferences.Set("jwt_token", result.Token);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] Login Exception: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Отримання списку товарів (GET api/v1/items)
        /// </summary>
        public async Task<List<ItemDto>?> GetItemsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                return await _httpClient.GetFromJsonAsync<List<ItemDto>>("api/v1/items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] GetItems Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Створення складської операції (POST api/v1/actions)
        /// </summary>
        public async Task<(bool Success, string Message)> CreateActionAsync(UserActionDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();

                // Формуємо payload з однаковими іменами для зчитування на сервері
                var payload = new
                {
                    itemName = dto.ItemName,
                    actionType = dto.ActionType,
                    quantity = dto.Quantity,
                    note = dto.Note
                };

                var response = await _httpClient.PostAsJsonAsync("api/v1/actions", payload);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Операцію успішно збережено у базі даних!");
                }

                string errorResponse = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrWhiteSpace(errorResponse) 
                    ? $"Сервер повернув код {response.StatusCode}." 
                    : errorResponse);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService Error]: {ex.Message}");
                return (false, $"Помилка з'єднання: {ex.Message}");
            }
        }

        /// <summary>
        /// Вихід із системи (видалення токена)
        /// </summary>
        public void Logout()
        {
            SecureStorage.Default.Remove("jwt_token");
            Preferences.Remove("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    // --- DTO Моделі ---

    public class LoginResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
