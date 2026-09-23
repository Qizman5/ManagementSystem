using System;
using ClientApp.Views;
using ClientApp.Views.Admin;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ClientApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Реєстрація маршрутів для навігації
            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            // Автоматична перевірка або примусове ввімкнення для тестування
            CheckAdminStatus();
        }

        // Перевіряємо збереженого користувача при запуску
        private async void CheckAdminStatus()
        {
            var email = await SecureStorage.Default.GetAsync("user_email");
            bool isAdmin = string.Equals(email, "arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase);
            
            // Якщо email не знайдено (перший запуск) або це адмін — показуємо
            SetAdminAccess(isAdmin);
        }

        // Вмикає або ховає Адмін-панель у бічному меню
        public void SetAdminAccess(bool isAdmin)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (AdminFlyoutItem != null)
                {
                    AdminFlyoutItem.IsVisible = isAdmin;
                }

                // Резервний пошук елемента в колекції Items
                foreach (var item in Items)
                {
                    if (item.Route == "AdminPage" || item.Title?.Contains("Адмін") == true)
                    {
                        item.IsVisible = isAdmin;
                    }
                }
            });
        }

        // Обробник натискання кнопки "Вийти з акаунту"
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                // Очищаємо дані сесії
                App.IsAuthenticated = false;
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_email");

                // Приховуємо адмінку для наступного користувача
                SetAdminAccess(false);
                FlyoutIsPresented = false;

                // Повертаємо на екран авторизації
                await GoToAsync("//LoginPage");
            }
        }
    }
}
