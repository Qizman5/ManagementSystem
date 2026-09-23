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

            // Реєструємо маршрути
            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            // Перевірка прав при запуску
            CheckAdminStatus();
        }

        private async void CheckAdminStatus()
        {
            var email = await SecureStorage.Default.GetAsync("user_email");
            bool isAdmin = string.Equals(email, "arotar2005@gmail.com", StringComparison.OrdinalIgnoreCase);
            SetAdminAccess(isAdmin);
        }

        public void SetAdminAccess(bool isAdmin)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (AdminFlyoutItem != null)
                {
                    AdminFlyoutItem.IsVisible = isAdmin;
                }

                foreach (var item in Items)
                {
                    if (item.Route == "AdminPage" || (item.Title != null && item.Title.Contains("Адмін")))
                    {
                        item.IsVisible = isAdmin;
                    }
                }
            });
        }

        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                App.IsAuthenticated = false;
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_email");

                SetAdminAccess(false);
                FlyoutIsPresented = false;

                await GoToAsync("//LoginPage");
            }
        }
    }
}
