using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModels;
using System.Data;
using ReadSphere.Data;
using Microsoft.AspNetCore.Identity;
using Models;
using Microsoft.EntityFrameworkCore;

namespace ReadSphere.Controllers
{
    public class AddNoteController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public AddNoteController(ILogger<HomeController> logger, ApplicationDBContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AddNotePage()
        {
            var model = new AddNoteViewModel();
            string userId = _userManager.GetUserId(User);

            if (userId == null)
                return RedirectToAction("Login", "Account");

            model.BooksList = await LoadUserBooks(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(AddNoteViewModel model)
        {
            string userId = _userManager.GetUserId(User);

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.BooksList = await LoadUserBooks(userId);
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
                cmd.Parameters.AddWithValue("@owner", userId);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while adding the note.");
                    model.BooksList = await LoadUserBooks(userId);
                    return View("AddNotePage", model);
                }
            }

            TempData["SuccessMessage"] = "Note added successfully!";
            return RedirectToAction("Index", "Home");
        }

        private async Task<List<AddNoteViewModel.BookOption>> LoadUserBooks(string userId)
        {
            var books = new List<AddNoteViewModel.BookOption>();
            var Books = await _context.Books
              .Where(b => b.Users.Any(u => u.Id == userId))
              .Select(b => new AddNoteViewModel.BookOption
              {
                  Id = b.Id,
                  Title = b.Title
              })
              .ToListAsync();

            foreach (var Book in Books)
            {
                books.Add(new AddNoteViewModel.BookOption
                {
                    Id = Book.Id,
                    Title = Book.Title
                });
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
