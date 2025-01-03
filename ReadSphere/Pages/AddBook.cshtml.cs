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
    public IFormFile cover_image { get; set; }





    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileName = Path.GetFileName(cover_image.FileName);

        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            cover_image.CopyTo(fileStream);
        }
        string imageUrl = "uploads/" + fileName;

        string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        Random random = new();

        int randomNumber = random.Next(0, 10000);
        string query = "INSERT INTO BOOK (Book_Id,Title, Author_Name, Publisher, Language, Cover_Image) " +
                       "VALUES (@id,@Title, @Author, @Publisher, @Language, @CoverImage)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@id", randomNumber);

            cmd.Parameters.AddWithValue("@Author", Author);
            cmd.Parameters.AddWithValue("@Publisher", Publisher);
            cmd.Parameters.AddWithValue("@Language", Language);
            cmd.Parameters.AddWithValue("@CoverImage", imageUrl);

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

        return RedirectToPage("/Book");
    }
}
