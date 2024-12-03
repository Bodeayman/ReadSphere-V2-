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


        }
        public void OnGet()
        {
            Clubs = new List<Club>();

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                // SQL query to insert a worker into the Workers table
                string query = "select * from club";

                SqlCommand cmd = new(query, connection);
                // Create a DataAdapter to fill the DataTable
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                try
                {
                    // Open the connection
                    connection.Open();

                    // Fill the DataTable with the data from the database
                    dataAdapter.Fill(dataTable);

                    // Process the data from the DataTable
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int Id = Convert.ToInt32(row["club_id"]);
                        string Name = row["club_name"].ToString() ?? "Unknown";
                        string Desc = row["club_description"].ToString() ?? "Unknown";


                        Club club = new Club();
                        club.Id = Id;
                        club.Name = Name;
                        club.Desc = Desc;
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
            // Retrieve the user ID from the cookie (replace "UserId" with your actual cookie name)
            string userId = Request.Cookies["user_id"]; // Adjust cookie name accordingly

            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case where the user ID is not available in the cookie
                // For example, redirect to the login page or show an error
                return RedirectToPage("/Login");
            }

            // Now you have both the user ID from the cookie and the book ID from the form
            // Proceed with the logic for adding the book for the user

            // Example: Save the book with the userId
            bool success = AddClubToUser(Convert.ToInt32(userId), ClubId);

            if (success)
            {
                // Redirect to a confirmation page or show a success message
                return RedirectToPage("/Index");
            }

            // If there was an error, handle it (e.g., show an error message)
            return Page();
        }

        // A sample method to simulate adding a book to the user's collection
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
                        // The club exists, now insert into the BooksPossess table
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