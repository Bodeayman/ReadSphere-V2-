using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using Microsoft.Data.SqlClient;
using System.Data;

public class Login : PageModel
{

    // Bind properties for form inputs
    [BindProperty]
    [Required(ErrorMessage = "Name is required.")]
    public string? email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    public string? password { get; set; }

    public string? ErrorMessage { get; set; }
    public bool is_admin { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            // Form validation failed
            email = "Invalid";
            return Page();
        }
        // Validate user credentials against the database
        if (ValidateUser(email!, password!))
        {
            // If successful, create a session or cookie and redirect
            Response.Cookies.Append(
                "User",
                email!,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true
                });
            Response.Cookies.Append(
    "is_admin",
    is_admin ? "admin" : "not",
    new CookieOptions
    {
        Expires = DateTimeOffset.Now.AddHours(1),
        HttpOnly = true,
        Secure = true
    });

            return RedirectToPage("Index");
        }

        // Invalid credentials
        ErrorMessage = "Invalid name or email.";
        return Page();
    }

    private bool ValidateUser(string email, string password)
    {
        is_admin = false;

        bool isValid = false;
        DataRow userRow = null;
        using (SqlConnection connection = new("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            string query = "SELECT * FROM [USER] WHERE Email = @email AND PASSWORD = @password";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            adapter.SelectCommand.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            adapter.SelectCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

            DataTable dataTable = new DataTable();

            try
            {
                connection.Open();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    userRow = dataTable.Rows[0];
                    Console.WriteLine($"Is_Admin value from database: {userRow["Is_Admin"]}");
                    is_admin = Convert.ToBoolean(userRow["Is_Admin"]);
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while retrieving user data.";
            }
        }

        return isValid;
    }
}
