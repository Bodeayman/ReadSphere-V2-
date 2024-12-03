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

        public string review_id { get; set; }
    }
    public void OnGet()
    {
        Books = new List<Book>();

        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            // SQL query to insert a worker into the Workers table
            string query = "select * from BOOK";

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
                    int Id = Convert.ToInt32(row["Book_Id"]);
                    string title = row["Title"].ToString();
                    string Author = row["Author_Name"].ToString();
                    string publisher = row["Publisher"].ToString();
                    string Language = row["Language"].ToString();
                    string cover_image = row["Cover_Image"].ToString();
                    string review_id = row["Review_Id"].ToString();
                    Book book = new Book();
                    book.Id = Id;
                    book.Title = title;
                    book.Author = Author;
                    book.Publisher = publisher;
                    book.Language = Language;
                    book.cover_image = cover_image;
                    book.review_id = review_id;
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
        bool success = AddBookToUser(Convert.ToInt32(userId), BookId);

        if (success)
        {
            // Redirect to a confirmation page or show a success message
            return RedirectToPage("/Index");
        }

        // If there was an error, handle it (e.g., show an error message)
        return Page();
    }

    // A sample method to simulate adding a book to the user's collection
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
                // The book exists, now insert into the BooksPossession table
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookId", bookId);
                command.ExecuteNonQuery();
                success = true;
            }
            else
            {
                // Handle the error, book not found
                Console.WriteLine("The specified book does not exist.");
            }
        }

        return success;
    }

}



