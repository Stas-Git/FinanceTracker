﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Expense_Tracker.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class AccountController : Controller
{
    private readonly UserService _userService; // Сервіс для роботи з користувачами
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("~/Views/User/Login.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _userService.ValidateUser(model.Email, model.Password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "/");
            }

            ModelState.AddModelError("", "Невірна пошта або пароль");
        }

        return View("~/Views/User/Login.cshtml", model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("~/Views/User/Register.cshtml");
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                Password = model.Password // Не забувайте хешувати пароль
            };

            var result = _userService.RegisterUser(user);

            if (result)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Не вдалося зареєструвати користувача.");
            }
        }

        return View("~/Views/User/Register.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при виході з акаунту: {ex.Message}");
        }
    }

    // Показ профілю користувача (GET)
    [HttpGet]
    public IActionResult Profile()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email); // Це поверне електронну пошту з claims


        var user = _userService.GetUserByEmail(userEmail);

        if (user == null)
        {
            return RedirectToAction("Login"); // If user is not found, redirect to login
        }

        var profileViewModel = new ProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email
        };

        return View("~/Views/User/Profile.cshtml", profileViewModel); // Return the profile view with the model
    }


    // Оновлення профілю користувача (POST)
    [HttpPost]
    public IActionResult Profile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Отримуємо поточного користувача з Email, який зберігається в Claims
            var userEmail = User.Identity.Name;

            var user = _userService.GetUserByEmail(userEmail);

            if (user == null)
            {
                return RedirectToAction("Login"); // Якщо користувач не знайдений
            }


            // Оновлюємо дані користувача
            user.UserName = model.UserName;
            user.Email = model.Email;

            // Не забувайте, якщо є зміни в паролі, потрібно їх хешувати та оновити
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = _userService.HashPassword(model.Password); // Хешуємо пароль перед збереженням
            }

            var result = _userService.UpdateUser(user);

            if (result)
            {
                // Оновлення інформації про користувача в Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                TempData["SuccessMessage"] = "Профіль успішно оновлено!";
                return RedirectToAction("Profile"); // Перенаправляємо на сторінку профілю після оновлення
            }

            ModelState.AddModelError("", "Не вдалося оновити профіль.");
        }

        return View("~/Views/User/Profile.cshtml", model);
    }
}