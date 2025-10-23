using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AddGoalsController : Controller
    {
        private readonly string connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // ✅ GET: Display the Add Goal page
        [HttpGet]
        public IActionResult AddGoalPage()
        {
            var model = new AddGoalViewModel();
            int userId = Convert.ToInt32(Request.Cookies["user_id"]);

            // Fetch user's books
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Book_id, Title FROM Book WHERE Book_id IN (SELECT BookId FROM BooksPossess WHERE OwnerId = @owner)";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@owner", userId);

                DataTable dt = new DataTable();
                connection.Open();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    model.BooksList.Add(new AddGoalViewModel.BookOption
                    {
                        Id = Convert.ToInt32(row["Book_id"]),
                        Title = row["Title"].ToString()
                    });
                }
            }

            return View("AddGoals", model);
        }

        // ✅ POST: Add a new reading goal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGoal(AddGoalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload book list to repopulate dropdown
                model.BooksList = GetUserBooks();
                return View("AddGoalPage", model);
            }

            int userId = Convert.ToInt32(Request.Cookies["user_id"]);
            Random random = new();
            int goalId = random.Next(0, 10000);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query1 = @"INSERT INTO reading_goal (goal_id, user_id, target_pages, book_id)
                                  VALUES (@id, @user_id, @target, @book)";
                string query2 = @"INSERT INTO notification (notification_id, message, date)
                                  VALUES (@id, @message, @date)";

                SqlCommand cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@id", goalId);
                cmd1.Parameters.AddWithValue("@user_id", userId);
                cmd1.Parameters.AddWithValue("@target", model.TargetPages);
                cmd1.Parameters.AddWithValue("@book", model.BookID);

                SqlCommand cmd2 = new SqlCommand(query2, connection);
                cmd2.Parameters.AddWithValue("@id", goalId);
                cmd2.Parameters.AddWithValue("@message", model.NotiMessage);
                cmd2.Parameters.AddWithValue("@date", model.NotiTime);

                try
                {
                    connection.Open();
                    cmd1.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding goal: {ex.Message}");
                    ModelState.AddModelError("", "Something went wrong while saving the goal.");
                    model.BooksList = GetUserBooks();
                    return View("AddGoals", model);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // Helper method to reload user’s book list (used after validation errors)
        private List<AddGoalViewModel.BookOption> GetUserBooks()
        {
            List<AddGoalViewModel.BookOption> books = new();
            int userId = Convert.ToInt32(Request.Cookies["user_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Book_id, Title FROM Book WHERE Book_id IN (SELECT BookId FROM BooksPossess WHERE OwnerId = @owner)";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@owner", userId);

                DataTable dt = new DataTable();
                connection.Open();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    books.Add(new AddGoalViewModel.BookOption
                    {
                        Id = Convert.ToInt32(row["Book_id"]),
                        Title = row["Title"].ToString()
                    });
                }
            }
            return books;
        }
    }
}
