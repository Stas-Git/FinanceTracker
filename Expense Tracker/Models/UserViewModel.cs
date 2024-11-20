using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ім'я користувача є обов'язковим")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Електронна пошта є обов'язковою")]
    [EmailAddress(ErrorMessage = "Некоректний формат електронної пошти")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Роль є обов'язковою")]
    public string Role { get; set; }

    [MinLength(3, ErrorMessage = "Пароль має містити щонайменше 3 символи")]
    public string? Password { get; set; }
}