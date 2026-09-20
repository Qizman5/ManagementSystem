using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Models
{
    [Table("Workers")] // Прив'язуємо модель до таблиці Workers
    public class User
    {
        [Key]
        [Column("ID")] // Колонка в БД називається ID
        public int Id { get; set; }

        [Column("Name")] // Колонка Name в БД використовується як Username/FullName
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        public string Username { get; set; } = string.Empty;

        [NotMapped] // Використовується в коді C# як псевдонім для Username (щоб не виникало помилок EF)
        public string FullName 
        { 
            get => Username; 
            set => Username = value; 
        }

        [Column("Email")]
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = string.Empty;

        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [NotMapped] // Якщо колонки Role немає в таблиці Workers, робимо значення за замовчуванням
        public string Role { get; set; } = "Worker";

        // Навігаційна властивість
        public ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
    }
}