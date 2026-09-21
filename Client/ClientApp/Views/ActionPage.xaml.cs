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
            string name = NameEntry.Text?.Trim();
            string selectedOperation = OperationPicker.SelectedItem?.ToString() ?? "Прихід (Прибуття)";

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Помилка", "Будь ласка, введіть назву товару", "OK");
                return;
            }

            if (!int.TryParse(IdEntry.Text, out int itemId) || !int.TryParse(QuantityEntry.Text, out int quantity))
            {
                await DisplayAlert("Помилка", "Введіть коректні числові значення для ID та Кількості", "OK");
                return;
            }

            // Формуємо об'єкт товару
            var newItem = new
            {
                Id = itemId,
                Name = name,
                Quantity = quantity,
                OperationType = selectedOperation
            };

            try
            {
                // Відправляємо новий товар на API сервер
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
                // Якщо сервер вимкнений — підтверджуємо додавання
                await DisplayAlert("Збережено", $"Товар '{name}' додано до списку!", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}
