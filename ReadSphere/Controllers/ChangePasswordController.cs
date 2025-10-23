using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly string connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // ✅ GET: /Account/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (Request.Cookies["user_id"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to change your password.";
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordViewModel());
        }

        // ✅ POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (Request.Cookies["user_id"] == null)
            {
                model.ErrorMessage = "You must be logged in to change your password.";
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            int userId = Convert.ToInt32(Request.Cookies["user_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkPasswordQuery = "SELECT Password FROM [User] WHERE User_id = @UserId";
                string updatePasswordQuery = "UPDATE [User] SET Password = @NewPassword WHERE User_id = @UserId";

                SqlCommand checkCmd = new SqlCommand(checkPasswordQuery, connection);
                checkCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                try
                {
                    connection.Open();
                    string storedPassword = checkCmd.ExecuteScalar() as string;

                    if (storedPassword == null || storedPassword != model.CurrentPassword)
                    {
                        model.ErrorMessage = "The current password is incorrect.";
                        return View(model);
                    }

                    SqlCommand updateCmd = new SqlCommand(updatePasswordQuery, connection);
                    updateCmd.Parameters.Add("@NewPassword", SqlDbType.NVarChar).Value = model.NewPassword;
                    updateCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    updateCmd.ExecuteNonQuery();

                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    model.ErrorMessage = "An error occurred while updating the password.";
                    return View(model);
                }
            }
        }
    }
}
