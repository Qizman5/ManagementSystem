using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Products")]
    public class Item
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("Discount", TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        // Обчислювальна властивість для Views
        [NotMapped]
        public decimal FinalPrice => Price * (1 - (Discount / 100m));
    }
}
