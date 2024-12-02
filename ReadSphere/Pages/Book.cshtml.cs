using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class BookModel : PageModel
{
    public List<Book> Books { get; set; }
    public int count { get; set; }
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }

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
                count = Books.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }


}
