namespace ServerApp.Models
{
    public class RegisterViewModel
    {
        public string Name { get; set; } = string.Empty; // 👈 Додайте це поле

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
