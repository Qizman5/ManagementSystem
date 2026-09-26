using System;
using System.Linq;
using ClientApp.Models;
using ClientApp.Services;
using Microsoft.Maui.Controls;

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
                // 1. Отримуємо товари для визначення реального ID
                var items = await _apiService.GetItemsAsync();
                
                // Шукаємо за назвою або беремо перший товар з бази даних
                var foundItem = items?.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)) 
                                ?? items?.FirstOrDefault();

                if (foundItem == null)
                {
                    await DisplayAlert("Помилка", "У базі даних немає жодного товару!", "OK");
                    return;
                }

                // 2. Формуємо тип операції
                string selectedType = OperationTypePicker.SelectedItem?.ToString() ?? "Income";
                string actionType = selectedType.Contains("Витрата") ? "Expense" :
                                    selectedType.Contains("Переміщення") ? "Transfer" : "Income";

                // 3. Відправляємо запит з валідним ItemId
                var dto = new UserActionDto
                {
                    UserId = 3,
                    ItemId = foundItem.Id,
                    Quantity = quantity,
                    ActionType = actionType,
                    Note = $"[Товар: {itemName}] {NoteEntry.Text?.Trim()}".Trim()
                };

                bool success = await _apiService.CreateActionAsync(dto);

                if (success)
                {
                    await DisplayAlert("Успіх", "Операцію успішно збережено!", "OK");
                    await Shell.Current.GoToAsync("//ItemsPage");
                }
                else
                {
                    await DisplayAlert("Помилка сервера", $"Сервер відхилив операцію для товару (ID: {foundItem.Id}). Перевірте наявність товару на складі.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Виникла помилка: {ex.Message}", "OK");
            }
        }
    }
}
