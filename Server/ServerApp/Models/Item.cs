using System;
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

        private int _quantity;
        [Required(ErrorMessage = "Вкажіть кількість")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        [Display(Name = "Кількість")]
        public int Quantity
        {
            get => _quantity;
            set => _quantity = Math.Abs(value);
        }

        private decimal _price;
        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(0.01, 999999999.99, ErrorMessage = "Ціна має бути більшою за 0 і не може бути від'ємною")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ціна (грн)")]
        public decimal Price
        {
            get => _price;
            set => _price = Math.Abs(value);
        }

        private decimal _discount;
        [Range(0, 100, ErrorMessage = "Знижка має бути в межах від 0% до 100%")]
        [Column(TypeName = "decimal(5,2)")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        [Display(Name = "Знижка (%)")]
        public decimal Discount
        {
            get => _discount;
            set
            {
                var absValue = Math.Abs(value);
                _discount = absValue > 100 ? 100 : absValue; // Перетворює -2% у 2%
            }
        }

        // Автоматично обчислювана ціна зі знижкою
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Ціна зі знижкою (грн)")]
        public decimal FinalPrice => Price * (1 - (Discount / 100m));
    }
}
