using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Вкажіть ім'я користувача")]
        [Display(Name = "Логін")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть Email")]
        [EmailAddress(ErrorMessage = "Некоректний формат пошти")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть пароль")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Пароль має містити щонайменше 4 символи")]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
