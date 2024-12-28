using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages
{

    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmNewPassword { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            if (Request.Cookies["user_id"] == null)
            {
                ErrorMessage = "You must be logged in to change your password.";
            }
        }

        public IActionResult OnPost()
        {
            if (Request.Cookies["user_id"] == null)
            {
                ErrorMessage = "You must be logged in to change your password.";
                return Page();
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "New passwords do not match.";
                return Page();
            }

            int userId = Convert.ToInt32(Request.Cookies["user_id"]);

            using (SqlConnection connection = new("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string checkPasswordQuery = "SELECT Password FROM [User] WHERE User_id = @UserId";
                SqlCommand checkPasswordCommand = new SqlCommand(checkPasswordQuery, connection);
                checkPasswordCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                try
                {
                    connection.Open();
                    string storedPassword = checkPasswordCommand.ExecuteScalar() as string;

                    if (storedPassword == null || storedPassword != CurrentPassword)
                    {
                        ErrorMessage = "The current password is incorrect.";
                        return Page();
                    }

                    string updatePasswordQuery = "UPDATE [User] SET Password = @NewPassword WHERE User_id = @UserId";
                    SqlCommand updatePasswordCommand = new SqlCommand(updatePasswordQuery, connection);
                    updatePasswordCommand.Parameters.Add("@NewPassword", SqlDbType.NVarChar).Value = NewPassword;
                    updatePasswordCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    updatePasswordCommand.ExecuteNonQuery();

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

}
