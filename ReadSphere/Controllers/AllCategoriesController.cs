using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModels;
using System.Data;

namespace ReadSphere.Controllers
{
    public class AllCategoriesController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public IActionResult Index()
        {
            var categories = new List<CategoryViewModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string categoryQuery = "SELECT * FROM category";
                string booksQuery = @"
                    SELECT book.Title 
                    FROM book_category 
                    JOIN book ON book.book_id = book_category.Book_Id 
                    WHERE book_category.Category_Id = @cat_id";

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(categoryQuery, connection);
                    DataTable categoryTable = new DataTable();
                    adapter.Fill(categoryTable);

                    foreach (DataRow row in categoryTable.Rows)
                    {
                        var category = new CategoryViewModel
                        {
                            Id = Convert.ToInt32(row["category_id"]),
                            Name = row["category_name"].ToString(),
                            Desc = row["category_desc"].ToString(),
                            Books = new List<string>()
                        };

                        SqlCommand cmd = new SqlCommand(booksQuery, connection);
                        cmd.Parameters.AddWithValue("@cat_id", category.Id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                category.Books.Add(reader["Title"].ToString()!);
                            }
                        }

                        categories.Add(category);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return View("AllCategories", categories);
        }

        [HttpPost]
        public IActionResult RedirectToAddBook(int CatId)
        {
            return RedirectToAction("AddBookCat", "Books", new { CatId = CatId });
        }
    }
}
