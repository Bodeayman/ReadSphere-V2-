using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using Microsoft.Data.SqlClient;
using System.Data;

public class Register : PageModel
{

    // Bind properties for form inputs
    [BindProperty]
    [Required(ErrorMessage = "Name is required.")]
    public string? email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "password is required.")]
    public string? password { get; set; }
    [BindProperty]
    [Required(ErrorMessage = "Name is required.")]
    public string? name { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Confirm Password is required.")]
    public string? confirmpassword { get; set; }

    public string? ErrorMessage { get; set; }

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


        using (SqlConnection connection = new("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            Random random = new();

            int randomNumber = random.Next(0, 10000);
            string query = "insert into [User]  values (@id, @name, @email, @password, 'no_image', 0)";

            SqlCommand command = new(query, connection);

            command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            command.Parameters.Add("@id", SqlDbType.NVarChar).Value = randomNumber;

            command.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                ErrorMessage = "An error occurred while validating your credentials.";
            }

        }
        return Page();
    }


}