using System;
using System.Linq;
using Microsoft.Maui.Controls;
using ClientApp.Models;
using ClientApp.Services;

namespace ClientApp.Views
{
    public partial class CreateOperationPage : ContentPage
    {
        private readonly ApiService _apiService;

        public CreateOperationPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        // 1. Валідатор для введення лише цифр у полі кількості
        private void OnNumericEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
                {
                    string cleanText = new string(e.NewTextValue.Where(char.IsDigit).ToArray());

                    if (long.TryParse(cleanText, out long parsedValue))
                    {
                        cleanText = parsedValue.ToString();
                    }
                    else if (cleanText == "0")
                    {
                        cleanText = string.Empty;
                    }

                    if (entry.Text != cleanText)
                    {
                        entry.Text = cleanText;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TextChange Error]: {ex.Message}");
            }
        }

        // 2. Обробка натискання кнопки «Зберегти операцію»
        private async void OnSaveOperationClicked(object sender, EventArgs e)
        {
            var itemName = ItemNameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(itemName))
            {
                await DisplayAlert("Увага", "Будь ласка, введіть назву товару!", "OK");
                return;
            }

            if (OperationTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Увага", "Будь ласка, оберіть тип операції!", "OK");
                return;
            }

            if (!int.TryParse(QuantityEntry.Text, out int quantity) || quantity <= 0)
            {
                await DisplayAlert("Увага", "Будь ласка, вкажіть кількість більше 0!", "OK");
                return;
            }

            try
            {
                // Визначаємо тип операції відповідно до обраного варіанта в Picker
                string selectedType = OperationTypePicker.SelectedItem?.ToString() ?? "Income";
                string actionType = selectedType.Contains("Списання") || selectedType.Contains("Витрата") ? "Outcome" :
                                    selectedType.Contains("Переміщення") ? "Transfer" : "Income";

                var dto = new UserActionDto
                {
                    UserId = 1,
                    ItemId = 0,
                    ItemName = itemName,
                    Quantity = quantity,
                    ActionType = actionType,
                    Note = NoteEntry.Text?.Trim() ?? string.Empty
                };

                // Відправляємо дані на сервер через ApiService
                var (success, message) = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await DisplayAlert("Успіх", string.IsNullOrWhiteSpace(message) ? "Операцію успішно збережено!" : message, "OK");

                    // Очищаємо поля після створення
                    ItemNameEntry.Text = string.Empty;
                    QuantityEntry.Text = string.Empty;
                    NoteEntry.Text = string.Empty;
                    OperationTypePicker.SelectedIndex = -1;

                    // Переходимо до списку товарів
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await DisplayAlert("Помилка", string.IsNullOrWhiteSpace(message) ? "Сервер відхилив операцію." : message, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка мережі", $"Не вдалося з'єднатися з сервером: {ex.Message}", "OK");
            }
        }
    }
}
