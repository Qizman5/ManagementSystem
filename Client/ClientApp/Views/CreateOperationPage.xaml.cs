using System;
using System.Linq;
using Microsoft.Maui.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class CreateOperationPage : ContentPage
    {
        public CreateOperationPage()
        {
            InitializeComponent();
            BindingContext = new ActionViewModel();
        }

        public CreateOperationPage(ActionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Завантаження товарів при відкритті сторінки
            if (BindingContext is ActionViewModel vm)
            {
                if (vm.Items == null || vm.Items.Count == 0)
                {
                    if (vm.LoadItemsCommand != null && vm.LoadItemsCommand.CanExecute(null))
                    {
                        await vm.LoadItemsCommand.ExecuteAsync(null);
                    }
                }
            }
        }

        // Захист від введення від'ємних чисел, мінусів, знаків та провідних нулів
        private void OnNumericEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
                {
                    // char.IsDigit залишає лише цифри (0-9), повністю видаляючи мінус, крапки та інші символи
                    string cleanText = new string(e.NewTextValue.Where(char.IsDigit).ToArray());

                    // Видаляємо провідні нулі (наприклад, "05" -> "5")
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
            try
            {
                if (BindingContext is ActionViewModel vm)
                {
                    // Перевірка 1: Чи обрано товар
                    if (vm.SelectedItem == null)
                    {
                        await DisplayAlert("Увага", "Будь ласка, оберіть товар зі списку!", "OK");
                        return;
                    }

                    // Перевірка 2: Вказана кількість більше 0
                    if (vm.Quantity <= 0)
                    {
                        await DisplayAlert("Увага", "Введіть кількість більше 0!", "OK");
                        return;
                    }

                    // Виконання збереження через ViewModel
                    if (vm.CreateActionCommand != null)
                    {
                        await vm.CreateActionCommand.ExecuteAsync(null);
                    }

                    await DisplayAlert("Успіх", "Операцію успішно збережено!", "OK");

                    // Повернення на сторінку товарів
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("//ItemsPage");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Save Operation Error]: {ex.Message}");
                await DisplayAlert("Помилка", $"Виникла помилка: {ex.Message}", "OK");
            }
        }
    }
}
