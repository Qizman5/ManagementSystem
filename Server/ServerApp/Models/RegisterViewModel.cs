using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Вкажіть повне ім'я")]
        [Display(Name = "Повне ім'я")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть логін")]
        [Display(Name = "Логін")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть Email")]
        [EmailAddress(ErrorMessage = "Некоректний формат пошти")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має містити щонайменше 8 символів")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "Пароль повинен містити принаймні одну велику літеру, одну малу літеру, цифру та спецсимвол (!@#$%^&*).")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
