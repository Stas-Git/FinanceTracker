using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        public string Password { get; set; }

    }
}
