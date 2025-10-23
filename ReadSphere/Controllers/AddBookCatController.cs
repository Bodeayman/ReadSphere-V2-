using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AddBookCatController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult Index(int catId)
        {
            var model = new AddBookCatViewModel { CategoryID = catId };
            model.BooksList = GetBooks();
            return View("AddBookCat", model);
        }

        [HttpPost]
        public IActionResult Index(AddBookCatViewModel model)
        {
            var userId = Request.Cookies["user_id"];
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            AddBookToCategory(Convert.ToInt32(userId), model.BookID, model.CategoryID);
            return RedirectToAction("Index", "Home");
        }

        private List<BookItem> GetBooks()
        {
            var list = new List<BookItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT Book_id, title FROM book";
            using var adapter = new SqlDataAdapter(query, connection);
            var table = new DataTable();
            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                list.Add(new BookItem
                {
                    Id = Convert.ToInt32(row["Book_id"]),
                    Title = row["title"].ToString()
                });
            }
            return list;
        }

        private void AddBookToCategory(int userId, int bookId, int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "INSERT INTO book_category (book_id, category_id) VALUES (@BookId, @CategoryId)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
