using System;

namespace ClientApp.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => _quantity = Math.Abs(value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set => _price = Math.Abs(value);
        }

        private decimal _discount;
        public decimal Discount
        {
            get => _discount;
            set
            {
                var absValue = Math.Abs(value);
                _discount = absValue > 100 ? 100 : absValue;
            }
        }

        public decimal FinalPrice => Price * (1 - (Discount / 100m));

        // Змінено на string для збігу з ItemsViewModel
        public string OperationType { get; set; } = "Income";
        public string Type { get; set; } = "Income";
    }
}
