using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;





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

        public List<User_Rating> Users_rating { get; set; }


    }

    public class User_Rating
    {
        public string? Name { get; set; }
        public string? comment { get; set; }
        public decimal rating { get; set; }
    }
    public int ItemId { get; set; }


    [BindProperty]
    public string? comment { get; set; }

    [BindProperty]

    public decimal? rating { get; set; }

    public void OnGet(int itemid)
    {
        ItemId = itemid;
        book = new Book();
        book.Users_rating = new List<User_Rating>();
        string query = " select* from book where Book_Id  = @item";
        string query4 = "select * from book ,book_review,review ,[user] where book.book_id = book_review.book_id and review.Review_Id = book_review.review_id and [User].user_id = review.user_id  and book.Book_Id = @bookID";

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
                User_Rating userRating = new User_Rating();
                userRating.Name = Convert.ToString(ratingrow["User_Name"]);
                userRating.rating = Convert.ToDecimal(ratingrow["Rating"]);
                userRating.comment = Convert.ToString(ratingrow["Description"]);
                book.Users_rating.Add(userRating);
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
                    book.avgRate = countRating != 0 ? rating / countRating : 0;

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}