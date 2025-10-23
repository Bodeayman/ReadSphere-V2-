using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

public class RegisterController : Controller
{
    private readonly string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

    [HttpGet]
    public IActionResult Register()
    {
        return View("Register", new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                // Check if email already exists
                string checkEmailQuery = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";
                using (var cmdCheck = new SqlCommand(checkEmailQuery, connection))
                {
                    cmdCheck.Parameters.AddWithValue("@Email", model.Email);
                    int emailCount = (int)cmdCheck.ExecuteScalar();
                    if (emailCount > 0)
                    {
                        ModelState.AddModelError("", "This email is already registered. Please use a different email.");
                        return View(model);
                    }
                }

                // Insert new user
                int userId = new Random().Next(0, 100000);
                int isAdmin = Request.Cookies["is_admin"] == "admin" ? 1 : 0;

                string insertQuery = "INSERT INTO [User] (User_id, User_name, Email, Password, is_admin) " +
                                     "VALUES (@id, @name, @email, @password, @isAdmin)";

                using (var cmdInsert = new SqlCommand(insertQuery, connection))
                {
                    cmdInsert.Parameters.AddWithValue("@id", userId);
                    cmdInsert.Parameters.AddWithValue("@name", model.Name);
                    cmdInsert.Parameters.AddWithValue("@email", model.Email);
                    cmdInsert.Parameters.AddWithValue("@password", model.Password);
                    cmdInsert.Parameters.AddWithValue("@isAdmin", isAdmin);

                    cmdInsert.ExecuteNonQuery();
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
}
