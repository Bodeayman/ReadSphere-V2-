using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AddCategoryController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("AddCategory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int randomCategoryId = new Random().Next(0, 10000);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO Category (Category_Id, category_name, category_desc)
                                     VALUES (@CategoryId, @Name, @Description)";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@CategoryId", randomCategoryId);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Description", model.Description);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to create category.");
                return View(model);
            }
        }
    }
}
