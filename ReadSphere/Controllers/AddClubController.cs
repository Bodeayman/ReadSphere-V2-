using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AddClubController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult AddClubPage()
        {
            return View("AddClub", new AddClubViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddClub(AddClubViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddClub", model);
            }

            Random random = new();
            int randomNumber = random.Next(0, 10000);

            string query = "INSERT INTO club (club_id, club_name, club_description) VALUES (@id, @name, @desc)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", randomNumber);
                cmd.Parameters.AddWithValue("@name", model.ClubName);
                cmd.Parameters.AddWithValue("@desc", model.ClubDescription);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    ModelState.AddModelError("", "Something went wrong while adding the club.");
                    return View("AddClub", model);
                }
            }

            // Optional success message
            TempData["SuccessMessage"] = "Club added successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
