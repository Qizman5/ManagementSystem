using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClientApp.Models;

namespace ClientApp.ViewModels
{
    public partial class AdminViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        [ObservableProperty]
        private ObservableCollection<UserActionLog> _userActions = new();

        public AdminViewModel()
        {
            LoadMockData();
        }

        private void LoadMockData()
        {
            Items = new ObservableCollection<Item>
            {
                new Item { Id = 1, Name = "Палета дерев'яна", Quantity = 50 },
                new Item { Id = 2, Name = "Коробка картонна (L)", Quantity = 120 },
                new Item { Id = 3, Name = "Стретч-плівка 20мкм", Quantity = 15 }
            };

            UserActions = new ObservableCollection<UserActionLog>
            {
                new UserActionLog { Username = "Operator1", ActionType = "Створення операції", Details = "Додано +20 Палета дерев'яна", Timestamp = DateTime.Now.AddMinutes(-15) },
                new UserActionLog { Username = "Admin", ActionType = "Редагування товару", Details = "Змінено кількість Коробка картонна", Timestamp = DateTime.Now.AddHours(-1) }
            };
        }
    }

    public class UserActionLog
    {
        public string Username { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
