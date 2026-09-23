using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = new ItemsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 1. Отримуємо email поточного користувача
            string? userEmail = await SecureStorage.Default.GetAsync("user_email");
            bool isAdmin = string.Equals(userEmail, "arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase);

            // 2. Видаляємо всі існуючі кнопки "Адмінка" з колекції (щоб уникнути дублювання)
            for (int i = ToolbarItems.Count - 1; i >= 0; i--)
            {
                if (ToolbarItems[i].Text != null && ToolbarItems[i].Text.Contains("Адмінка"))
                {
                    ToolbarItems.RemoveAt(i);
                }
            }

            // 3. Динамічно додаємо кнопку "Адмінка" ТІЛЬКИ якщо користувач — arotar2005@gmail.com
            if (isAdmin)
            {
                ToolbarItems.Insert(0, new ToolbarItem
                {
                    Text = "🛡️ Адмінка",
                    Priority = 0,
                    Command = new Command(async () => await Shell.Current.GoToAsync("//AdminPage"))
                });
            }

            if (BindingContext is ItemsViewModel vm)
            {
                if (vm.LoadItemsCommand != null && vm.LoadItemsCommand.CanExecute(null))
                {
                    await vm.LoadItemsCommand.ExecuteAsync(null);
                }
            }
        }

        // Кнопка "Вийти"
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                App.IsAuthenticated = false;
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_email");

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetAdminAccess(false);
                }

                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        // Кнопка "Створити нову операцію"
        public async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreateOperationPage");
        }
    }
}
