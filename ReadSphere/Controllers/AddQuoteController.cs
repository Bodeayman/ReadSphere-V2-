using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AddQuoteController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult AddQuotePage()
        {
            if (!UserIsLoggedIn())
                return RedirectToAction("Login", "Account");

            var model = new AddQuoteViewModel
            {
                BooksList = LoadUserBooks()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuote(AddQuoteViewModel model)
        {
            if (!UserIsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.BooksList = LoadUserBooks();
                return View("AddQuotePage", model);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO Quote (Quote_Id, Book_Id, quote_text, Added_date, Page_number, owner_quote_id)
                                     VALUES (@id, @book_id, @quote_text, @added_date, @page, @owner)";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    int randomId = new Random().Next(1, 10000);

                    cmd.Parameters.AddWithValue("@id", randomId);
                    cmd.Parameters.AddWithValue("@book_id", model.BookID);
                    cmd.Parameters.AddWithValue("@quote_text", model.QuoteText);
                    cmd.Parameters.AddWithValue("@added_date", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@page", model.PageNumber);
                    cmd.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Quote added successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to add quote.");
                model.BooksList = LoadUserBooks();
                return View("AddQuotePage", model);
            }
        }

        private List<AddQuoteViewModel.BookItem> LoadUserBooks()
        {
            var books = new List<AddQuoteViewModel.BookItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Book_id, Title 
                                 FROM Book 
                                 WHERE Book_id IN (SELECT BookId FROM BooksPossess WHERE OwnerId = @owner)";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

                DataTable table = new DataTable();
                connection.Open();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    books.Add(new AddQuoteViewModel.BookItem
                    {
                        Id = Convert.ToInt32(row["Book_id"]),
                        Title = row["Title"].ToString()
                    });
                }
            }

            return books;
        }

        private bool UserIsLoggedIn() => Request.Cookies["user_id"] != null;
    }
}
