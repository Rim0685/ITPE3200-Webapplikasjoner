// The Controllers folder contains classes that handle web requests.
// This controller handles user registration, login and logout.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    // Receives Identity services and the logger through dependency injection.
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Creates a user with support for profile information.
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(
                    user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(
                        user, isPersistent: false);

                    _logger.LogInformation(
                        "A user registered successfully.");

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty, error.Description);
                }

                _logger.LogWarning("User registration failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "An error occurred during registration.");

                ModelState.AddModelError(
                    string.Empty,
                    "Registration could not be completed. Please try again.");
            }
        }

        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Uses a local return address or redirects to the home page.
        string targetUrl =
            !string.IsNullOrEmpty(model.ReturnUrl)
            && Url.IsLocalUrl(model.ReturnUrl)
                ? model.ReturnUrl
                : Url.Action("Index", "Home") ?? "~/";

        if (ModelState.IsValid)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "A user logged in successfully.");

                    return Redirect(targetUrl);
                }

                _logger.LogWarning("A login attempt failed.");

                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "An error occurred during login.");

                ModelState.AddModelError(
                    string.Empty,
                    "Login could not be completed. Please try again.");
            }
        }

        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation(
                "A user logged out successfully.");

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "An error occurred during logout.");

            return StatusCode(
                500,
                "Logout could not be completed. Please try again.");
        }
    }
}