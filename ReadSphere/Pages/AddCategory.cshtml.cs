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
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            Random random = new Random();
            int randomCategoryId = random.Next(0, 10000);

            string insertCategoryQuery = "INSERT INTO Category (Category_Id, category_name, category_desc) " +
                                         "VALUES (@CategoryId, @Name, @Description)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(insertCategoryQuery, connection);

                cmd.Parameters.AddWithValue("@CategoryId", randomCategoryId);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Description", Description);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return Page();
                }
            }

            return RedirectToPage("/Index");
        }
    }
}
