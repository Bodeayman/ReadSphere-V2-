using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using ReadSphere.Data;
using System.Data;
using ViewModels;

public class RegisterController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult RegisterPage()
    {
        Console.WriteLine("Nothing returning yet");
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

            User newRegisteredUser = new User
            {
                Email = model.Email,
                UserName = model.Name,
                Role = Enums.UserRoles.User,


            };
            IdentityResult UserRegisterd = await _userManager.CreateAsync(newRegisteredUser, model.Password!);
            if (UserRegisterd.Succeeded)
            {
                {
                    string role = "User";

                    // Ensure the role exists
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));

                    // Assign the role
                    await _userManager.AddToRoleAsync(newRegisteredUser, role);

                    return RedirectToAction("Index", "Home");
                }


            }
            else
            {
                foreach (IdentityError Result in UserRegisterd.Errors)
                {
                    Console.WriteLine(Result.Description + " " + Result.Code);
                }
                model.ErrorMessage = "Invalid registering";
                return RedirectToAction("RegisterPage", new RegisterViewModel());
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine(ex);
            ModelState.AddModelError("", "An error occurred while processing your request.");
            return View(model);
        }
    }
}