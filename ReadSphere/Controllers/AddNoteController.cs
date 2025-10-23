using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModels;
using System.Data;

namespace ReadSphere.Controllers
{
    public class AddNoteController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult AddNotePage()
        {
            var model = new AddNoteViewModel();
            int? userId = GetUserIdFromCookie();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            model.BooksList = LoadUserBooks(userId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNote(AddNoteViewModel model)
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.BooksList = LoadUserBooks(userId.Value);
                return View("AddNotePage", model);
            }

            Random random = new();
            int randomNumber = random.Next(0, 10000);

            string query = @"INSERT INTO Note 
                            (Note_Id, Book_Id, Note_Text, Added_date, Page_number, owner_note_id)
                             VALUES (@id, @book_id, @note_text, @added_date, @page, @owner)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", randomNumber);
                cmd.Parameters.AddWithValue("@book_id", model.BookID);
                cmd.Parameters.AddWithValue("@note_text", model.NoteText);
                cmd.Parameters.AddWithValue("@added_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@page", model.PageNumber);
                cmd.Parameters.AddWithValue("@owner", userId.Value);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while adding the note.");
                    model.BooksList = LoadUserBooks(userId.Value);
                    return View("AddNotePage", model);
                }
            }

            TempData["SuccessMessage"] = "Note added successfully!";
            return RedirectToAction("Index", "Home");
        }

        private List<AddNoteViewModel.BookOption> LoadUserBooks(int userId)
        {
            var books = new List<AddNoteViewModel.BookOption>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Book_id, title FROM book WHERE Book_id IN (SELECT BookId FROM booksPossess WHERE ownerid = @owner)";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@owner", userId);

                DataTable dt = new();
                connection.Open();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    books.Add(new AddNoteViewModel.BookOption
                    {
                        Id = Convert.ToInt32(row["Book_id"]),
                        Title = row["title"].ToString() ?? "Unknown"
                    });
                }
            }
            return books;
        }

        private int? GetUserIdFromCookie()
        {
            var cookie = Request.Cookies["user_id"];
            if (int.TryParse(cookie, out int id))
                return id;
            return null;
        }
    }
}
