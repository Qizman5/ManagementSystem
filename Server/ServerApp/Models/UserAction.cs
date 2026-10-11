using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Actions")]
    public class UserAction
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("WorkerID")]
        public int WorkerId { get; set; }

        [Column("ProductID")]
        public int ProductId { get; set; }

        [Required]
        [Column("ActionType")]
        public string ActionType { get; set; } = string.Empty;

        [Column("Quantity")]
        public int Quantity { get; set; } = 1;

        [Column("PriceAtMoment", TypeName = "decimal(18,2)")]
        public decimal PriceAtMoment { get; set; }

        [Column("Note")]
        public string? Note { get; set; }

        [Column("ActionDate")]
        public DateTime ActionDate { get; set; } = DateTime.Now;

        // --- Псевдоніми та зв'язки для сумісності з EF Core та Controllers ---
        [NotMapped]
        public int UserId 
        { 
            get => WorkerId; 
            set => WorkerId = value; 
        }

        [NotMapped]
        public int ItemId 
        { 
            get => ProductId; 
            set => ProductId = value; 
        }

        [ForeignKey("WorkerId")]
        public Worker? Worker { get; set; }

        [NotMapped]
        public Worker? User 
        { 
            get => Worker; 
            set => Worker = value; 
        }

        [ForeignKey("ProductId")]
        public Item? Item { get; set; }
    }
}
