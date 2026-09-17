using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути більшою за 0")]
        public decimal Price { get; set; }
    }
}