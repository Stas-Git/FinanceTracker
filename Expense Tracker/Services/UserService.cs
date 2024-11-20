using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly ApplicationDbContext _context; // Використовуємо DbContext

    public UserService(ApplicationDbContext context)
    {
        _context = context; // Призначаємо DbContext
    }

    // Метод для реєстрації користувача
    public bool RegisterUser(User user)
    {
        // Перевірка, чи існує вже користувач з таким email
        if (_context.Users.Any(u => u.Id == user.Id))
        {
            return false; // Користувач з таким email вже існує
        }

        // Хешування пароля (необхідно для безпеки)
        user.Password = HashPassword(user.Password); // Хешуємо пароль перед збереженням

        _context.Users.Add(user); // Додаємо користувача в контекст
        _context.SaveChanges(); // Збережемо зміни в базі

        return true; // Успішно зареєстровано
    }

    public User GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public User? ValidateUser(string email, string password)
    {
        string hashedPassword = HashPassword(password); // Хешуємо введений пароль
        return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword); // Знайти користувача по email та паролю
    }

    // Метод для отримання користувача по email
    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email); // Пошук користувача по email
    }

    // Метод для оновлення даних користувача
    public bool UpdateUser(User updatedUser)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == updatedUser.Email);
        if (user == null) return false; // Якщо користувача не знайдено

        user.UserName = updatedUser.UserName; // Оновлюємо ім'я
        user.Password = updatedUser.Password != user.Password ? HashPassword(updatedUser.Password) : user.Password; // Якщо пароль змінено, хешуємо його
        _context.Users.Update(user); // Оновлюємо користувача в контексті
        _context.SaveChanges(); // Зберігаємо зміни в базі

        return true; // Дані успішно оновлено
    }

    // Хешування пароля
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); // Хешуємо пароль
        return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // Перетворюємо в строку
    }
}
