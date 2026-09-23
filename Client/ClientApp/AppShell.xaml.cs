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

            // Реєстрація маршрутів для навігації
            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
        }

        // Виправлено тип параметра: ShellNavigatingEventArgs замість ShellNavigatingArgs
        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // Виправлено CS8600: додано '?' для безпечної обробки null
            string? token = await SecureStorage.Default.GetAsync("jwt_token");
            bool isAuthenticated = !string.IsNullOrEmpty(token);

            // Список захищених сторінок
            var protectedPages = new[] { "ItemsPage", "CreateOperationPage", "AdminPage" };

            // Перевірка шляху
            string targetLocation = args.Target?.Location?.OriginalString ?? string.Empty;

            if (!isAuthenticated && protectedPages.Any(page => targetLocation.Contains(page)))
            {
                args.Cancel();

                await DisplayAlert("Доступ обмежено", "Будь ласка, спочатку увійдіть або зареєструйтеся в системі.", "OK");

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
