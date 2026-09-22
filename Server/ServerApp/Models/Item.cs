using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва товару є обов'язковою")]
        [Display(Name = "Назва товару")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Опис")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть кількість")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        [Display(Name = "Кількість")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Ціна має бути більшою за 0 і не може бути від'ємною")]
        [Display(Name = "Ціна (грн)")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Знижка має бути в межах від 0% до 100%")]
        [Display(Name = "Знижка (%)")]
        public decimal Discount { get; set; } = 0;

        // Автоматично обчислювана ціна зі знижкою (не зберігається окремим стовпчиком у БД)
        [NotMapped]
        [Display(Name = "Ціна зі знижкою (грн)")]
        public decimal FinalPrice => Price * (1 - (Discount / 100m));
    }
}
