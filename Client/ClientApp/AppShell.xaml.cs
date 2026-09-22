using ClientApp.Views;
using ClientApp.Views.Admin;
using Microsoft.Maui.Controls;

namespace ClientApp
{
    public partial class AppShell : Shell
    {
        // Додано '?' для позначення, що поле може містити null
        private FlyoutItem? _adminFlyoutItem;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CreateOperationPage), typeof(CreateOperationPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
        }

        // Динамічне управління видимістю адмінки
        public void SetAdminAccess(bool isAdmin)
        {
            if (isAdmin)
            {
                // Якщо пункт ще не додано — додаємо в бічне меню
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
                // Якщо звичайний користувач — видаляємо адмінку з меню
                if (_adminFlyoutItem != null && Items.Contains(_adminFlyoutItem))
                {
                    Items.Remove(_adminFlyoutItem);
                    _adminFlyoutItem = null;
                }
            }
        }
    }
}
