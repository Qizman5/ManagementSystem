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
        private FlyoutItem? _adminFlyoutItem;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

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
                if (isAdmin)
                {
                    if (_adminFlyoutItem == null)
                    {
                        _adminFlyoutItem = new FlyoutItem
                        {
                            Title = "🛡️ Адмін-панель",
                            Route = "AdminPage",
                            Items =
                            {
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(AdminPage)),
                                    Route = "AdminPage"
                                }
                            }
                        };

                        // Вставляємо кнопку прямо в меню
                        Items.Add(_adminFlyoutItem);
                    }
                }
                else
                {
                    if (_adminFlyoutItem != null && Items.Contains(_adminFlyoutItem))
                    {
                        Items.Remove(_adminFlyoutItem);
                        _adminFlyoutItem = null;
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
