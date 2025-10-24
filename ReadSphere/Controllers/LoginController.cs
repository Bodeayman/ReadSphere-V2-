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
        {
            return View(model);
        }

        try
        {
            await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            return RedirectToAction("Index", "Home");
        }


        catch (Exception err)
        {
            model.ErrorMessage = "Invalid email or password.";
            return View(model);
        }
        // Invalid credentials

    }


}
