using System;
using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть назву товару")]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        public decimal Price { get; set; }

        // Дозволяємо значення від -100% (знижка) до +100% (націнка)
        [Range(-100, 100, ErrorMessage = "Значення має бути від -100% до +100%")]
        public decimal Discount { get; set; }

        // Фінальна ціна:
        // Якщо Discount = -5 (знижка 5%), FinalPrice = Price * (1 - 0.05) = Price * 0.95
        // Якщо Discount = +5 (націнка 5%), FinalPrice = Price * (1 + 0.05) = Price * 1.05
        public decimal FinalPrice => Price * (1m + (Discount / 100m));
    }
}
