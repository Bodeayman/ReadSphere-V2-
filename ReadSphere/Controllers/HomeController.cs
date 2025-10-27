using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using ReadSphere.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;




namespace ReadSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly string connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        private int GetCount(string query)
        {
            int count = 0;
            using SqlConnection connection = new(connectionString);
            try
            {
                SqlDataAdapter da = new(query, connection);
                DataTable dt = new();
                connection.Open();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                    count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error counting: {ex.Message}");
            }
            return count;
        }



        [HttpGet]
        public async IActionResult Index(string? searchQuery)
        {
            if (Request.Cookies["user_id"] == null)
                return View(new DashboardViewModel());

            DashboardViewModel vm = new DashboardViewModel();

            // Fill totals
            vm.TotalBooks = GetCount("SELECT COUNT(*) FROM book");
            vm.TotalClubs = GetCount("SELECT COUNT(*) FROM club");
            vm.TotalQuotes = GetCount("SELECT COUNT(*) FROM quote");
            vm.TotalNotes = GetCount("SELECT COUNT(*) FROM note");
            vm.TotalUsers = GetCount("SELECT COUNT(*) FROM [User]");

            vm.SearchQuery = searchQuery;

            using SqlConnection connection = new(connectionString);
            connection.Open();


            // Books
            /*
            _context.Books.Where()
            */
            var userId = _userManager.GetUserId(User); // gets logged-in user's ID

            var books = await _context.Books
                .Where(b => b.Users.Any(u => u.Id == userId)) // filter books related to that user
                .Include(b => b.Users)
                .ToListAsync();

            foreach (Book Book in books)
            {
                var title = Book.Title.ToString()!;
                var author = Book.Author.ToString()!;

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var regex = new Regex(searchQuery, RegexOptions.IgnoreCase);
                    if (!regex.IsMatch(title) && !regex.IsMatch(author))
                        continue;
                }

                Book book = new()
                {
                    Id = Convert.ToInt32(Book.Id),
                    Title = title,
                    Author = author,
                    Publisher = Book.Publisher,
                    Language = Book.Language,
                    CoverImage = Book.CoverImage
                };

                vm.MyBooks.Add(book);
            }

            // Clubs

            var Clubs = await _context.Clubs
                .Where(b => b.Users.Any(C => C.Id == userId)) // filter books related to that user
                .Include(b => b.Users)
                .ToListAsync();
            foreach (Club Club in Clubs)
            {

                vm.MyClubs.Add(new Club
                {
                    Users = new List<User>(),
                    Name = Club.Name,
                    Description = Club.Description
                });
            }

            // Quotes
            string queryQuotes = "SELECT * FROM quote JOIN book ON quote.book_id = book.Book_Id WHERE owner_quote_id = @owner";

            foreach (DataRow row in dt.Rows)
            {
                vm.MyQuotes.Add(new Quote
                {
                    User = new User(),
                    Book = new Book(),
                    QuoteText = "This is a test Quote",
                    CreatedAt = DateTime.Today,
                    PageNumber = 1
                });
            }

            // Notes
            string queryNotes = "SELECT * FROM note JOIN book ON note.book_id = book.Book_Id WHERE owner_note_id = @owner";
            da = new(queryNotes, connection);
            da.SelectCommand.Parameters.AddWithValue("@owner", userId);
            dt = new();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                vm.MyNotes.Add(new Note
                {
                    NoteText = row["note_text"].ToString(),
                    Author = row["Author_Name"].ToString(),
                    Book = new Book(),
                    DateTime = Convert.ToDateTime(row["added_date"]),
                    PageNumber = Convert.ToInt32(row["page_number"])
                });
            }

            // Notifications (Goals)
            string queryGoals = "SELECT * FROM notification, reading_goal, book WHERE notification.goal_id = reading_goal.goal_id AND book.Book_Id = reading_goal.book_id AND reading_goal.user_id = @owner";
            da = new(queryGoals, connection);
            da.SelectCommand.Parameters.AddWithValue("@owner", userId);
            dt = new();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var notif = new Notification
                {
                    DueDate = DateTime.Today,
                    Message = row["notification_message"].ToString(),
                    Goal = new Goal
                    {
                        Book = new Book()
                    },
                    Pages = Convert.ToInt32(row["target_pages"])
                };
                if (DateTime.Now > notif.DueDate)
                    vm.DueGoing.Add(notif);
                else
                    vm.Ongoing.Add(notif);
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("user_id");
            Response.Cookies.Delete("User");
            Response.Cookies.Delete("is_admin");
            return RedirectToAction("Index");
        }
    }
}
