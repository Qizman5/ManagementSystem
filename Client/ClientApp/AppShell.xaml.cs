using ClientApp.Views;
using ClientApp.Views.Admin;
using Microsoft.Maui.Controls;

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
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            string targetLocation = args.Target?.Location?.OriginalString ?? string.Empty;

            // 1. Пропускаємо без перевірок, якщо перехід іде на LoginPage або під час скидання навігації
            if (targetLocation.Contains("LoginPage") || string.IsNullOrEmpty(targetLocation))
            {
                return;
            }

            // 2. Перевіряємо наявність токена
            string? token = await SecureStorage.Default.GetAsync("jwt_token");
            bool isAuthenticated = !string.IsNullOrEmpty(token);

            var protectedPages = new[] { "ItemsPage", "CreateOperationPage", "AdminPage" };

            // 3. Блокуємо ТІЛЬКИ якщо токена дійсно немає і сторінка захищена
            if (!isAuthenticated && protectedPages.Any(page => targetLocation.Contains(page)))
            {
                args.Cancel();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Доступ обмежено", "Будь ласка, спочатку увійдіть або зареєструйтеся в системі.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                });
            }
        }

        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                SecureStorage.Default.Remove("jwt_token");
                SetAdminAccess(false);

                FlyoutIsPresented = false;
                await Shell.Current.GoToAsync("//LoginPage");
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
