using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ReadSphere
{
    public class CreateCategoryModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public void OnGet()
        {
            // This method runs when the page is loaded (GET request)
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // If the model state is invalid, return to the same page
            }

            string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            // Randomly generate Category ID (like you did with review_id, although in a real scenario, the database should auto-generate it)
            Random random = new Random();
            int randomCategoryId = random.Next(0, 10000); // Use a more sophisticated method in real applications, like using an auto-incremented column

            // SQL query to insert the category into the database
            string insertCategoryQuery = "INSERT INTO Category (Category_Id, category_name, category_desc) " +
                                         "VALUES (@CategoryId, @Name, @Description)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(insertCategoryQuery, connection);

                cmd.Parameters.AddWithValue("@CategoryId", randomCategoryId); // Category ID (could be auto-generated in a real scenario)
                cmd.Parameters.AddWithValue("@Name", Name); // Category Name
                cmd.Parameters.AddWithValue("@Description", Description); // Category Description

                try
                {
                    connection.Open();
                    // Execute the command to insert the category into the database
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return Page(); // If there's an error, return to the same page
                }
            }

            return RedirectToPage("/Index"); // Redirect to the Index page (or wherever you want to go after adding the category)
        }
    }
}
