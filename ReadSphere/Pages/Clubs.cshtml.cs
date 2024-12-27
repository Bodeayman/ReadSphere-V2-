using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages
{
    public class ClubsModel : PageModel
    {

        public List<Club> Clubs { get; set; }
        public int count { get; set; }
        public int ClubId { get; set; }


        public class Club
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }

            public List<String> users { get; set; }


        }
        public void OnGet()
        {
            Clubs = new List<Club>();

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "select * from club";
                string query2 = "select  [User].User_Name  from [User] , club , CLUBS_JOINED where club.club_id = clubs_joined.club_id and clubs_joined.user_id = [User].user_id and club.club_id = @club_id    ";

                SqlCommand cmd = new(query, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                DataTable dataTable = new DataTable();





                try
                {
                    connection.Open();

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int Id = Convert.ToInt32(row["club_id"]);
                        string Name = row["club_name"].ToString() ?? "Unknown";
                        string Desc = row["club_description"].ToString() ?? "Unknown";

                        Club club = new Club();
                        club.Id = Id;
                        club.Name = Name;
                        club.Desc = Desc;
                        club.users = new List<String>();
                        SqlDataAdapter clubs_users_table_adapter = new SqlDataAdapter(query2, connection);


                        DataTable clubs_users_table = new DataTable();

                        clubs_users_table_adapter.SelectCommand.Parameters.AddWithValue("@club_id", Id);

                        clubs_users_table_adapter.Fill(clubs_users_table);

                        foreach (DataRow club_user in clubs_users_table.Rows)
                        {
                            club.users.Add(Convert.ToString(club_user["user_name"]));
                        }
                        Clubs.Add(club);
                    }
                    count = Clubs.Count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public IActionResult OnPost(int ClubId)
        {
            Console.WriteLine($"Received ClubId: {ClubId}");
            string userId = Request.Cookies["user_id"];

            if (string.IsNullOrEmpty(userId))
            {

                return RedirectToPage("/Login");
            }


            bool success = AddClubToUser(Convert.ToInt32(userId), ClubId);

            if (success)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        private bool AddClubToUser(int userId, int ClubId)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "INSERT INTO clubs_joined (user_id, club_id) VALUES (@UserId, @ClubId)";
                SqlCommand command = new SqlCommand(query, connection);

                // Ensure the club exists in the CLUB table first
                string checkClubExistsQuery = "SELECT COUNT(*) FROM dbo.Club WHERE Club_id = @Club_id";
                SqlCommand checkCommand = new SqlCommand(checkClubExistsQuery, connection);

                // Properly declare and assign parameters
                checkCommand.Parameters.AddWithValue("@Club_id", ClubId);  // Correct parameter name is @Club_id
                checkCommand.Parameters.AddWithValue("@UserId", userId);   // Add UserId parameter as well for insert query

                connection.Open();

                // Execute the check command to get the count
                var result = checkCommand.ExecuteScalar();
                if (result != null)
                {
                    int clubCount = Convert.ToInt32(result); // Safely convert to int

                    Console.WriteLine($"Club count: {clubCount}"); // Debug log

                    if (clubCount > 0)
                    {
                        command.Parameters.AddWithValue("@UserId", userId);  // Ensure you add the parameter for the insert query
                        command.Parameters.AddWithValue("@ClubId", ClubId);  // Ensure you add the parameter for the insert query
                        command.ExecuteNonQuery();
                        success = true;
                    }
                    else
                    {
                        // Handle the error: club not found
                        Console.WriteLine("The specified club does not exist.");
                    }
                }
                else
                {
                    // Handle the case where the result is null
                    Console.WriteLine("Error: Club query did not return a valid result.");
                }
            }

            return success;
        }

    }
}