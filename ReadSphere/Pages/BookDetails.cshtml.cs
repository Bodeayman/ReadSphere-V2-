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
        public double avgRate { get; set; }
        public string? cover_image { get; internal set; }
        public string? Language { get; internal set; }
    }
    public int ItemId { get; set; }

    public void OnGet(int itemid)
    {
        ItemId = itemid;
        book = new Book();

        string query = " select* from book where Book_Id  = @item";
        string query4 = "select * from book join review on book.Review_Id = review.Review_Id where Book_Id = @bookID";


        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@item", itemid);

            DataTable dataTable = new DataTable();
            DataTable ratingforbook = new DataTable();

            SqlDataAdapter ratingTable = new SqlDataAdapter(query4, connection);
            ratingTable.SelectCommand.Parameters.AddWithValue("@bookID", itemid);
            double countRating = 0;
            double rating = 0;
            ratingTable.Fill(ratingforbook);
            foreach (DataRow ratingrow in ratingforbook.Rows)
            {
                rating += Convert.ToInt32(ratingrow["Rating"]);
                countRating++;
            }


            try
            {
                connection.Open();

                dataAdapter.Fill(dataTable);


                foreach (DataRow row in dataTable.Rows)
                {
                    int Id = Convert.ToInt32(row["Book_Id"]);
                    string title = row["Title"].ToString();

                    string Author = row["Author_Name"].ToString() ?? "";
                    string publisher = row["Publisher"].ToString() ?? "Unknown";
                    string Language = row["Language"].ToString() ?? "Unknown";
                    string cover_image = row["Cover_Image"].ToString() ?? "Unknown";

                    Console.WriteLine(Language);
                    book.Id = Id;
                    book.Title = title;
                    book.Author = Author;
                    book.Publisher = publisher;
                    book.Language = Language;
                    book.cover_image = cover_image;
                    book.avgRate = rating;

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}