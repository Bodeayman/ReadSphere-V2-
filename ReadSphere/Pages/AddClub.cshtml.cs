using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


public class AddClubModel : PageModel
{
    [BindProperty]
    public string club_name { get; set; }
    [BindProperty]
    public string club_desc { get; set; }
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // SQL query to insert a new book into the BOOK table
        Random random = new();

        int randomNumber = random.Next(0, 10000);
        string query = "INSERT INTO club (club_id,club_name,club_description) " +
                       "VALUES (@id,@name, @desc)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            // parameters to avoid SQL injection
            cmd.Parameters.AddWithValue("@desc", club_desc);
            cmd.Parameters.AddWithValue("@id", randomNumber);

            cmd.Parameters.AddWithValue("@name", club_name);


            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Page();
            }
        }

        return RedirectToPage("/Index");// redirection to books page
    }
}
