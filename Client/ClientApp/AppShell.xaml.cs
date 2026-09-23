using System;
using ClientApp.Views;
using ClientApp.Views.Admin;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ClientApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }

        // Обробник кнопка "Вийти з акаунту"
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                // 1. Очищаємо дані сесії
                App.IsAuthenticated = false;
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_email");

                // 2. Ховаємо адмін-панель для наступного входу
                SetAdminAccess(false);

                // 3. Закриваємо бокове меню
                FlyoutIsPresented = false;

                // 4. Безпечно повертаємо на екран авторизації
                await GoToAsync("//LoginPage");
            }
        }

        // Вмикає або ховає пункт меню залежно від того, чи це адмін
        public void SetAdminAccess(bool isAdmin)
        {
            if (AdminFlyoutItem != null)
            {
                AdminFlyoutItem.IsVisible = isAdmin;
            }
        }
    }
}
