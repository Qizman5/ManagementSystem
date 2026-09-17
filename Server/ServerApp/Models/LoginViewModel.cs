using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Вкажіть ім'я користувача")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}