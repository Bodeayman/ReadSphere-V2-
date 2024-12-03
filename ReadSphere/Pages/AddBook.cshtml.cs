using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

public class AddBookModel : PageModel
{
    [BindProperty]
    public string Title { get; set; }
    [BindProperty]
    public string Author { get; set; }

    [BindProperty]
    public string Publisher { get; set; }
    [BindProperty]
    public string Language { get; set; }

    [BindProperty]
    public string cover_image { get; set; }

    [BindProperty]
    public string review_id { get; set; }



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
        string query = "INSERT INTO BOOK (Book_Id,Title, Author_Name, Publisher, Language, Cover_Image, Review_Id) " +
                       "VALUES (@id,@Title, @Author, @Publisher, @Language, @CoverImage, @ReviewId)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            // parameters to avoid SQL injection
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@id", randomNumber);

            cmd.Parameters.AddWithValue("@Author", Author);
            cmd.Parameters.AddWithValue("@Publisher", Publisher);
            cmd.Parameters.AddWithValue("@Language", Language);
            cmd.Parameters.AddWithValue("@CoverImage", cover_image);
            cmd.Parameters.AddWithValue("@ReviewId", review_id);

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

        return RedirectToPage("/Book");// redirection to books page
    }
}
