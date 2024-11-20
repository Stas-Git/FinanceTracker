using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public AdminController(ApplicationDbContext context, UserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public IActionResult AddOrEditUser(int? id)
        {
            if (id == null) // Додавання нового користувача
            {
                return View(new UserViewModel());
            }

            var user = _userService.GetUserById(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddOrEditUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0) // Створення нового користувача
                {
                    var newUser = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Role = model.Role
                    };

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        newUser.Password = _userService.HashPassword(model.Password);
                    }

                    try
                    {
                        var result = _userService.RegisterUser(newUser);
                        if (result)
                        {
                            TempData["SuccessMessage"] = "Користувача успішно створено!";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Користувач з таким email вже існує.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Помилка при створенні користувача: " + ex.Message);
                    }
                }
                else // Оновлення існуючого користувача
                {
                    var user = _userService.GetUserById(model.Id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Оновлення полів
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.Role = model.Role;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        user.Password = _userService.HashPassword(model.Password);
                    }

                    try
                    {
                        var result = _userService.UpdateUser(user);
                        if (result)
                        {
                            TempData["SuccessMessage"] = "Дані користувача успішно оновлено!";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Не вдалося оновити користувача.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Помилка при оновленні користувача: " + ex.Message);
                    }
                }
            }

            // Повертаємо модель назад у форму, якщо є помилки
            return View(model);
        }

    }
}