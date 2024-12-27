using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class BookModel : PageModel
{
    public List<Book> Books { get; set; }
    public int count { get; set; }
    public int BookId { get; set; }


    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        public string Language { get; set; }

        public string cover_image { get; set; }

    }
    public void OnGet()
    {
        Books = new List<Book>();

        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            string query = "select * from BOOK ";
            SqlCommand cmd = new(query, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();

            try
            {
                connection.Open();

                dataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    int Id = Convert.ToInt32(row["Book_Id"]);
                    string title = row["Title"].ToString();
                    string Author = row["Author_Name"].ToString();
                    string publisher = row["Publisher"].ToString();
                    string Language = row["Language"].ToString();
                    string cover_image = row["Cover_Image"].ToString();
                    Book book = new Book();
                    book.Id = Id;
                    book.Title = title;
                    book.Author = Author;
                    book.Publisher = publisher;
                    book.Language = Language;
                    book.cover_image = cover_image;
                    Books.Add(book);
                }
                ViewData["AllBooks"] = Books;
                count = Books.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public IActionResult OnPost(int BookId)
    {
        Console.WriteLine($"Received BookId: {BookId}");
        string userId = Request.Cookies["user_id"];

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Login");
        }


        bool success = AddBookToUser(Convert.ToInt32(userId), BookId);

        if (success)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    private bool AddBookToUser(int userId, int bookId)
    {
        bool success = false;

        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            string query = "INSERT INTO BooksPossess (OwnerId, BookId) VALUES (@UserId, @BookId)";
            SqlCommand command = new SqlCommand(query, connection);

            // Ensure the book exists in the BOOK table first
            string checkBookExistsQuery = "SELECT COUNT(*) FROM dbo.BOOK WHERE Book_Id = @BookId";
            SqlCommand checkCommand = new SqlCommand(checkBookExistsQuery, connection);
            checkCommand.Parameters.AddWithValue("@BookId", bookId);

            connection.Open();
            int bookCount = (int)checkCommand.ExecuteScalar();
            Console.WriteLine($"Book count: {bookCount}"); // Debug log


            if (bookCount > 0)
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookId", bookId);
                command.ExecuteNonQuery();
                success = true;
            }
            else
            {
                Console.WriteLine("The specified book does not exist.");
            }
        }

        return success;
    }

}



