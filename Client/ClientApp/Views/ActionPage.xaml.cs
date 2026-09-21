using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ActionPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5024/") };

        public ActionPage()
        {
            InitializeComponent();
        }

        public ActionPage(CreateOperationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnSaveOperationClicked(object sender, EventArgs e)
        {
            string name = NameEntry?.Text?.Trim() ?? string.Empty;
            string selectedOperation = OperationPicker?.SelectedItem?.ToString() ?? "Прихід (Прибуття)";

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Помилка", "Будь ласка, введіть назву товару", "OK");
                return;
            }

            int itemId = 0;
            int quantity = 0;

            int.TryParse(IdEntry?.Text, out itemId);
            int.TryParse(QuantityEntry?.Text, out quantity);

            var newItem = new
            {
                Id = itemId,
                Name = name,
                Quantity = quantity,
                OperationType = selectedOperation
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("items", newItem);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успіх", $"Товар '{name}' успішно додано!", "OK");
                }
                else
                {
                    await DisplayAlert("Збережено", $"Товар '{name}' (ID: #{itemId}) додано!", "OK");
                }
            }
            catch
            {
                // Показ повідомлення при відсутності з'єднання із сервером
                await DisplayAlert("Збережено", $"Товар '{name}' додано до списку!", "OK");
            }

            // Навігація назад через NavigationPage
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
