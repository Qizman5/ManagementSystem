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

            // Отримуємо токен з безпечного сховища
            string? token = await SecureStorage.Default.GetAsync("jwt_token");
            bool isAuthenticated = !string.IsNullOrEmpty(token);

            // Отримуємо точний шлях, куди прямує користувач
            string targetLocation = args.Target?.Location?.OriginalString ?? string.Empty;

            // Якщо користувач прямує на сторінку авторизації — НЕ блокуємо її ніколи
            if (targetLocation.Contains("LoginPage"))
            {
                return;
            }

            // Список захищених сторінок
            var protectedPages = new[] { "ItemsPage", "CreateOperationPage", "AdminPage" };

            // Перевіряємо, чи намагається неавторизований користувач зайти на захищену сторінку
            if (!isAuthenticated && protectedPages.Any(page => targetLocation.Contains(page)))
            {
                args.Cancel();

                // Викликуємо сповіщення асинхронно у головному потоці, щоб не блокувати навігаційний стек
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
