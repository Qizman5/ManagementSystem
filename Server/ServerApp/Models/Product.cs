using ServerApp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [NotMapped]
        public int Id { get => ID; set => ID = value; }

        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }

        [NotMapped]
        public decimal FinalPrice => Discount switch
        {
            > 0 and <= 100 => Math.Round(Price * (1m - Discount / 100m), 2),
            > 100 => Math.Max(0m, Price - Discount),
            _ => Price
        };

        public static implicit operator Item?(Product? product)
        {
            if (product == null) return null;
            return new Item
            {
                Id = product.ID,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price,
                Discount = product.Discount
            };
        }
    }
}
