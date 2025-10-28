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
using System.Xml;




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
        public async Task<IActionResult> Index(string? searchQuery)
        {
            Console.WriteLine("Activating the program");
            if (User.Identity != null)
            {
                Console.WriteLine($"Authenticated: {User.Identity.IsAuthenticated}");
                Console.WriteLine($"User: {User.Identity.Name}");
            }
            else
            {
                Console.WriteLine("No user identity found.");
            }


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
            Console.WriteLine("The number of books is " + books.Count);
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

                vm.MyBooks.Add(Book);
            }

            // Clubs

            var Clubs = await _context.Clubs
                .Where(b => b.Users.Any(C => C.Id == userId)) // filter books related to that user
                .Include(b => b.Users)
                .ToListAsync();
            foreach (Club Club in Clubs)
            {

                vm.MyClubs.Add(Club);
            }

            // Quotes
            string queryQuotes = "SELECT * FROM quote JOIN book ON quote.book_id = book.Book_Id WHERE owner_quote_id = @owner";
            var Quotes = await _context.Quotes
              .Where(b => b.UserId == userId) // filter books related to that user
              .Include(b => b.Book)
              .ToListAsync();
            foreach (Quote Quote in Quotes)
            {
                vm.MyQuotes.Add(Quote);
            }

            // Notes
            string queryNotes = "SELECT * FROM note JOIN book ON note.book_id = book.Book_Id WHERE owner_note_id = @owner";
            var Notes = await _context.Notes
           .Where(b => b.UserId == userId) // filter books related to that user
           .Include(b => b.Book)
           .ToListAsync();
            foreach (Note Note in Notes)
            {
                vm.MyNotes.Add(Note);
            }
            /*
            // Notifications (Goals)
            string queryGoals = "SELECT * FROM notification, reading_goal, book WHERE notification.goal_id = reading_goal.goal_id AND book.Book_Id = reading_goal.book_id AND reading_goal.user_id = @owner";
            var Notifications =
           await _context.Notifications
           .Where(b => b.GoalId == userId) // filter books related to that user
           .ToListAsync();

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
            */

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
