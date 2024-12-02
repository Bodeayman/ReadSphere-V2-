using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

public class AddBookModel : PageModel
{
    [BindProperty]
    public Book Book { get; set; }

    public class Book
    {
        public int Id { get; set; } // can be generated in DB
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string cover_image { get; set; }
        public string review_id { get; set; }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // SQL query to insert a new book into the BOOK table
        string query = "INSERT INTO BOOK (Title, Author_Name, Publisher, Language, Cover_Image, Review_Id) " +
                       "VALUES (@Title, @Author, @Publisher, @Language, @CoverImage, @ReviewId)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            // parameters to avoid SQL injection
            cmd.Parameters.AddWithValue("@Title", Book.Title);
            cmd.Parameters.AddWithValue("@Author", Book.Author);
            cmd.Parameters.AddWithValue("@Publisher", Book.Publisher);
            cmd.Parameters.AddWithValue("@Language", Book.Language);
            cmd.Parameters.AddWithValue("@CoverImage", Book.cover_image);
            cmd.Parameters.AddWithValue("@ReviewId", Book.review_id);

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

        return RedirectToPage("/Books");// redirection to books page
    }
}
