using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using ReadSphere.Data;
using System.Data;
using ViewModels;

public class LoginController : Controller
{

    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDBContext _context;

    public LoginController(ApplicationDBContext context, SignInManager<User> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View("Login", new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Your account is locked.");
            else if (result.IsNotAllowed)
                ModelState.AddModelError(string.Empty, "You are not allowed to sign in. Confirm your email first.");
            else if (result.RequiresTwoFactor)
                ModelState.AddModelError(string.Empty, "Two-factor authentication is required.");
            else
            {
                Console.WriteLine("Another error lol");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            }

            return View("Login", model);

        }
        catch (Exception)
        {
            model.ErrorMessage = "An error occurred during login.";

            return View("Login", model);

        }
    }


}
