using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ClientApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // http://10.0.2.2:5024/ - для Android Emulator
        // http://localhost:5024/ - для Windows / iOS Simulator
        private const string BaseUrl = "http://10.0.2.2:5024/"; 

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
        /// Авторизація користувача та збереження токена
        /// </summary>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", result.Token);
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
        /// Отримання списку товарів (GET api/items)
        /// </summary>
        public async Task<List<ItemDto>?> GetItemsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                return await _httpClient.GetFromJsonAsync<List<ItemDto>>("api/items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] GetItems Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Створення складської операції (POST api/actions)
        /// </summary>
        public async Task<bool> CreateActionAsync(UserActionDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync("api/actions", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] CreateAction Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Вихід із системи (видалення збереженого токена)
        /// </summary>
        public void Logout()
        {
            SecureStorage.Default.Remove("jwt_token");
            Preferences.Remove("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UserActionDto
    {
        public int ItemId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
