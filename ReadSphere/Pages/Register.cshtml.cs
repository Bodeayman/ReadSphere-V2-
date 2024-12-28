using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using Microsoft.Data.SqlClient;
using System.Data;

public class Register : PageModel
{


    [BindProperty]
    public string? email { get; set; }

    [BindProperty]
    public string? password { get; set; }

    [BindProperty]
    public string? name { get; set; }

    [BindProperty]
    public string? confirmpassword { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {

        if (password != confirmpassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        using (SqlConnection connection = new("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            string checkEmailQuery = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";
            SqlCommand checkEmailCommand = new SqlCommand(checkEmailQuery, connection);
            checkEmailCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

            try
            {
                connection.Open();
                int emailCount = (int)checkEmailCommand.ExecuteScalar();
                if (emailCount > 0)
                {
                    ErrorMessage = "This email is already registered. Please use a different email.";
                    return Page();
                }


                Random random = new();
                int randomNumber = random.Next(0, 100000);
                string query = "";
                if (Request.Cookies["is_admin"] == "admin")
                {
                    query = "INSERT INTO [User] (User_id, User_name, Email, Password, is_admin) VALUES (@id, @name, @email, @password, 1)";

                }
                else
                {
                    query = "INSERT INTO [User] (User_id, User_name, Email, Password, is_admin) VALUES (@id, @name, @email, @password, 0)";

                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@id", SqlDbType.Int).Value = randomNumber;
                command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                command.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                command.ExecuteNonQuery();

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your request.";
                Console.WriteLine(ex);
                return Page();
            }
        }
    }
}





