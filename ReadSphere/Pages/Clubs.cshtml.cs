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
    }
}
