using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

// Book model definition




// Page model for book details
public class BookDetailsModel : PageModel
{
    public Book book { get; set; }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Publisher { get; internal set; }
        public string review_id { get; set; }
        public string? cover_image { get; internal set; }
        public string? Language { get; internal set; }
    }
    public int ItemId { get; set; }

    public void OnGet(int itemid)
    {
        ItemId = itemid;
        book = new Book();

        string query = " select* from book where Book_Id  = @item";

        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            // SQL query to insert a worker into the Workers table

            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@item", itemid);

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

                    string Author = row["Author_Name"].ToString() ?? "";
                    string publisher = row["Publisher"].ToString() ?? "Unknown";
                    string Language = row["Language"].ToString() ?? "Unknown";
                    string cover_image = row["Cover_Image"].ToString() ?? "Unknown";
                    string review_id = row["Review_Id"].ToString() ?? "Unknown";
                    Console.WriteLine(Language);
                    book.Id = Id;
                    book.Title = title;
                    book.Author = Author;
                    book.Publisher = publisher;
                    book.Language = Language;
                    book.cover_image = cover_image;
                    book.review_id = review_id;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}