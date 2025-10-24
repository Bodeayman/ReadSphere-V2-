using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;
using System.Data;
using System.Text.RegularExpressions;

namespace ReadSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult Index(string? searchQuery)
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

            int userId = Convert.ToInt32(Request.Cookies["user_id"]);

            // Books
            string queryBooks = "SELECT * FROM book WHERE book_id IN (SELECT BookId FROM booksPossess WHERE ownerid = @owner)";
            SqlDataAdapter da = new(queryBooks, connection);
            da.SelectCommand.Parameters.AddWithValue("@owner", userId);
            DataTable dt = new();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var title = row["Title"].ToString()!;
                var author = row["Author_Name"].ToString()!;

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var regex = new Regex(searchQuery, RegexOptions.IgnoreCase);
                    if (!regex.IsMatch(title) && !regex.IsMatch(author))
                        continue;
                }

                Book book = new()
                {
                    Id = Convert.ToInt32(row["Book_Id"]),
                    Title = title,
                    Author = author,
                    Publisher = row["Publisher"].ToString(),
                    Language = row["Language"].ToString(),
                    CoverImage = row["Cover_Image"].ToString()
                };

                vm.MyBooks.Add(book);
            }

            // Clubs
            string queryClubs = "SELECT * FROM club WHERE club_id IN (SELECT club_id FROM clubs_joined WHERE user_id = @owner)";
            da = new(queryClubs, connection);
            da.SelectCommand.Parameters.AddWithValue("@owner", userId);
            dt = new();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {

                vm.MyClubs.Add(new Club
                {
                    Users = new List<User>(),
                    Name = row["club_name"].ToString(),
                    Description = row["club_description"].ToString()
                });
            }

            // Quotes
            string queryQuotes = "SELECT * FROM quote JOIN book ON quote.book_id = book.Book_Id WHERE owner_quote_id = @owner";
            da = new(queryQuotes, connection);
            da.SelectCommand.Parameters.AddWithValue("@owner", userId);
            dt = new();
            da.Fill(dt);
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
