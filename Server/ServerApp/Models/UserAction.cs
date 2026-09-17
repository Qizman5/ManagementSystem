using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Actions")]
    public class UserAction
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public string ActionType { get; set; } = string.Empty;
        public int QuantityChanged { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}