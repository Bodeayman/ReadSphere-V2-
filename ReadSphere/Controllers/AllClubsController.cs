using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using ViewModels;

namespace ReadSphere.Controllers
{
    public class AllClubsController : Controller
    {
        private readonly string connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // GET: /Clubs
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new ClubViewModel();
            viewModel.Clubs = GetAllClubs();
            viewModel.Count = viewModel.Clubs.Count;
            return View("AllClubs", viewModel);
        }

        // POST: /Clubs/Join
        [HttpPost]
        public IActionResult Join(int clubId)
        {
            string userId = Request.Cookies["user_id"];

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            bool success = AddClubToUser(Convert.ToInt32(userId), clubId);

            if (success)
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Index");
        }

        // Helper: Get all clubs
        private List<Club> GetAllClubs()
        {
            List<Club> clubs = new();

            using SqlConnection connection = new(connectionString);
            string query = "SELECT * FROM club";
            string usersQuery = @"
                SELECT [User].User_Name 
                FROM [User], club, CLUBS_JOINED 
                WHERE club.club_id = clubs_joined.club_id 
                  AND clubs_joined.user_id = [User].user_id 
                  AND club.club_id = @club_id";

            try
            {
                connection.Open();
                SqlDataAdapter adapter = new(query, connection);
                DataTable table = new();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    var club = new Club
                    {
                        Id = Convert.ToInt32(row["club_id"]),
                        Name = row["club_name"].ToString() ?? "Unknown",
                        Desc = row["club_description"].ToString() ?? "Unknown",
                        users = new List<string>()
                    };

                    SqlDataAdapter usersAdapter = new(usersQuery, connection);
                    usersAdapter.SelectCommand.Parameters.AddWithValue("@club_id", club.Id);

                    DataTable usersTable = new();
                    usersAdapter.Fill(usersTable);

                    foreach (DataRow userRow in usersTable.Rows)
                    {
                        club.users.Add(userRow["user_name"].ToString() ?? "Unknown");
                    }

                    clubs.Add(club);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching clubs: {ex.Message}");
            }

            return clubs;
        }

        // Helper: Add club to user
        private bool AddClubToUser(int userId, int clubId)
        {
            using SqlConnection connection = new(connectionString);
            string checkClubQuery = "SELECT COUNT(*) FROM dbo.Club WHERE Club_id = @Club_id";
            string insertQuery = "INSERT INTO clubs_joined (user_id, club_id) VALUES (@UserId, @ClubId)";

            try
            {
                connection.Open();

                using SqlCommand checkCmd = new(checkClubQuery, connection);
                checkCmd.Parameters.AddWithValue("@Club_id", clubId);
                int clubExists = (int)checkCmd.ExecuteScalar();

                if (clubExists > 0)
                {
                    using SqlCommand insertCmd = new(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@ClubId", clubId);
                    insertCmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    Console.WriteLine("Club does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding club: {ex.Message}");
            }

            return false;
        }
    }
}
