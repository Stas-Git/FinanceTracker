using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models // Змініть на ваш актуальний простір імен
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Ім'я користувача є обов'язковим.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; }
    }
}
