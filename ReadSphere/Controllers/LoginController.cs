using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

public class LoginController : Controller
{
    private readonly string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

    [HttpGet]
    public IActionResult Index()
    {
        return View("Login", new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (ValidateUser(model.Email!, model.Password!, out bool isAdmin, out string userId))
        {
            Response.Cookies.Append(
                "User",
                model.Email!,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true
                });

            Response.Cookies.Append(
                "is_admin",
                isAdmin ? "admin" : "not",
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true
                });

            Response.Cookies.Append(
                "user_id",
                userId,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true
                });

            return RedirectToAction("Index", "Home");
        }

        // Invalid credentials
        model.ErrorMessage = "Invalid email or password.";
        return View(model);
    }

    private bool ValidateUser(string email, string password, out bool isAdmin, out string userId)
    {
        isAdmin = false;
        userId = string.Empty;

        using (SqlConnection connection = new(connectionString))
        {
            string query = "SELECT * FROM [USER] WHERE Email = @Email AND PASSWORD = @Password";
            SqlDataAdapter adapter = new(query, connection);
            adapter.SelectCommand.Parameters.AddWithValue("@Email", email);
            adapter.SelectCommand.Parameters.AddWithValue("@Password", password);

            DataTable dataTable = new();

            try
            {
                connection.Open();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    isAdmin = Convert.ToBoolean(row["Is_Admin"]);
                    userId = Convert.ToString(row["User_Id"])!;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user: {ex.Message}");
            }
        }

        return false;
    }
}
