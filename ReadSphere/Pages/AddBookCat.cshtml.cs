using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ReadSphere.Pages
{
    public class AddBookCat : PageModel
    {
        public class newBook
        {
            public string Title { get; set; }
            public int Id { get; set; }
        }

        public List<newBook> bookslist { get; set; }

        // Define the BookID property to bind the selected book
        [BindProperty]
        public int BookID { get; set; }

        // The CategoryID is now being passed directly from the URL
        [BindProperty(SupportsGet = true)]  // Bind CategoryID from the query string
        public int CategoryID { get; set; }

        public void OnGet(int CatId)
        {
            // Assign the CategoryID from the query string
            CategoryID = CatId;

            bookslist = new List<newBook>();

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                // Query to get books
                string booksQuery = "SELECT Book_id, title FROM book";
                SqlDataAdapter booksDataAdapter = new SqlDataAdapter(booksQuery, connection);
                DataTable booksDataTable = new DataTable();

                try
                {
                    connection.Open();
                    booksDataAdapter.Fill(booksDataTable);

                    foreach (DataRow row in booksDataTable.Rows)
                    {
                        bookslist.Add(new newBook
                        {
                            Id = Convert.ToInt32(row["Book_id"]),
                            Title = row["title"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public IActionResult OnPost(int BookID, int CategoryID)
        {
            Console.WriteLine($"Received BookID: {BookID}");
            Console.WriteLine($"Received CategoryID: {CategoryID}");

            string userId = Request.Cookies["user_id"];

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            // Add the book-category relationship into the database
            bool success = AddBookToCategory(Convert.ToInt32(userId), BookID, CategoryID);

            if (success)
            {
                return RedirectToPage("/Index"); // Redirect to another page (e.g., index) after success
            }

            return RedirectToPage("/Index"); // Return to the current page with the form if something goes wrong
        }

        private bool AddBookToCategory(int userId, int bookId, int categoryId)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "INSERT INTO book_category (book_id, category_id) VALUES (@BookId, @CategoryId)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@BookId", bookId);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
    }

}
