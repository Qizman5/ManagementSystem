using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Workers")]
    public class Worker
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Username")]
        public string? Username { get; set; }

        [Column("FullName")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("Role")]
        public string Role { get; set; } = "Worker";

        // --- Псевдоніми для сумісності зі старим кодом ---
        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(Username) ? Username : (!string.IsNullOrEmpty(Name) ? Name : Email);

        // Навігаційна властивість зв'язку з операціями
        public ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
    }
}
