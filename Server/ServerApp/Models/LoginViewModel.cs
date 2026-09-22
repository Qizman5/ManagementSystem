using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле 'Логін' є обов'язковим для заповнення.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Пароль' є обов'язковим для заповнення.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
