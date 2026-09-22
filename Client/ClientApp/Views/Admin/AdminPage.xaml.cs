using System;
using Microsoft.Maui.Controls;
using ClientApp.Models;
using ClientApp.ViewModels;

namespace ClientApp.Views.Admin
{
    public partial class AdminPage : ContentPage
    {
        public AdminPage()
        {
            InitializeComponent();
            BindingContext = new AdminViewModel();
        }

        private void OnShowItemsClicked(object sender, EventArgs e)
        {
            ItemsSection.IsVisible = true;
            LogsSection.IsVisible = false;
        }

        private void OnShowLogsClicked(object sender, EventArgs e)
        {
            ItemsSection.IsVisible = false;
            LogsSection.IsVisible = true;
        }

        private async void OnEditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Item item)
            {
                string result = await DisplayPromptAsync("Редагування", $"Введіть нову кількість для '{item.Name}':", initialValue: item.Quantity.ToString(), keyboard: Keyboard.Numeric);
                if (int.TryParse(result, out int newQty))
                {
                    item.Quantity = newQty;
                    await DisplayAlert("Успіх", "Кількість оновлено!", "OK");
                }
            }
        }

        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Item item)
            {
                bool confirm = await DisplayAlert("Підтвердження", $"Ви дійсно хочете видалити товар '{item.Name}'?", "Так", "Ні");
                if (confirm && BindingContext is AdminViewModel vm)
                {
                    vm.Items.Remove(item);
                    await DisplayAlert("Видалено", "Товар успішно видалено!", "OK");
                }
            }
        }
    }
}