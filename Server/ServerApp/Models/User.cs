using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Логін обов'язковий")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "ПІБ обов'язкове")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обов'язковий"), EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Worker"; // Worker, Manager, Admin

        // Навігаційна властивість для зв'язку з таблицею дій
        public ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
    }
}