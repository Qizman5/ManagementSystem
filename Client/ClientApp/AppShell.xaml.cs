using System;
using ClientApp.Views;
using ClientApp.Views.Admin;
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
        }

        // Безпечний вихід з акаунту
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                // 1. Очищаємо сесію
                App.IsAuthenticated = false;
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_email");

                // 2. Скидаємо права адміна
                SetAdminAccess(false);

                // 3. Закриваємо бокове меню
                FlyoutIsPresented = false;

                // 4. Безпечно повертаємо на екран авторизації
                await GoToAsync("//LoginPage");
            }
        }

        public void SetAdminAccess(bool isAdmin)
        {
            if (isAdmin)
            {
                if (_adminFlyoutItem == null)
                {
                    _adminFlyoutItem = new FlyoutItem
                    {
                        Title = "🛡️ Адмін-панель",
                        Route = nameof(AdminPage),
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(AdminPage)),
                                Route = nameof(AdminPage)
                            }
                        }
                    };

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
        }
    }
}
