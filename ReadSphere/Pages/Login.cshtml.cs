using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

public class Login : PageModel
{
    // Try to put BindProperty with everything 
    [BindProperty]
    public string? Age { get; set; }
    [BindProperty]

    public string? Email { get; set; }
    [BindProperty]

    public string? Name { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {   //Pass this attribute to the Index Model
        // Create a cookie
        Response.Cookies.Append(
                    "User",
                    Name,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddHours(1),
                        HttpOnly = true,
                        Secure = true
                    });

        return RedirectToPage("Index", new { name = Name, age = Age });
    }
}