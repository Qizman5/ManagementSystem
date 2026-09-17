using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Actions")]
    public class UserAction
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        [Required]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }
    }
}