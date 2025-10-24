using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using ReadSphere.Data;
using System.Data;
using ViewModels;

public class RegisterController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDBContext _context;

    public RegisterController(ApplicationDBContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("Register", new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {

            int isAdmin = Request.Cookies["is_admin"] == "admin" ? 1 : 0;
            User newRegisteredUser = new User
            {
                Email = model.Email,
                UserName = model.Name,
                Role = Enums.UserRoles.User,

            };
            IdentityResult UserRegisterd = await _userManager.CreateAsync(newRegisteredUser, model.Password!);
            if (UserRegisterd.Succeeded)
            {
                Console.WriteLine("You registerd Successfully you dump bas*ard");
            }
            return RedirectToAction("Index", "Home");
        }


        catch (Exception ex)
        {
            Console.WriteLine(ex);
            ModelState.AddModelError("", "An error occurred while processing your request.");
            return View(model);
        }
    }
}