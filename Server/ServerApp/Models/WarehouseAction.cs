using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class WarehouseAction
    {
        public int Id { get; set; }

        [Required]
        public string ActionType { get; set; } = "Receipt"; // Receipt (Находження), Shipment (Списання)

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}