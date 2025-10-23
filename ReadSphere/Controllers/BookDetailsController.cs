using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ViewModels;

public class BookDetailsController : Controller
{
    private string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

    [HttpGet]
    public IActionResult Details(int id)
    {
        Console.WriteLine("Working on the details page here");
        var model = new BookDetailsViewModel();
        Console.WriteLine("ID is {0}", id);
        using (var connection = new SqlConnection(connectionString))
        {
            string queryBook = "SELECT * FROM book WHERE Book_Id = @item";
            string queryRatings = @"
                SELECT u.User_Name, r.Rating, r.Description
                FROM book b
                JOIN book_review br ON b.Book_Id = br.Book_Id
                JOIN review r ON br.Review_Id = r.Review_Id
                JOIN [User] u ON r.User_Id = u.User_Id
                WHERE b.Book_Id = @bookID";

            var bookTable = new DataTable();
            var ratingTable = new DataTable();

            var bookAdapter = new SqlDataAdapter(queryBook, connection);
            bookAdapter.SelectCommand.Parameters.AddWithValue("@item", id);

            var ratingAdapter = new SqlDataAdapter(queryRatings, connection);
            ratingAdapter.SelectCommand.Parameters.AddWithValue("@bookID", id);

            connection.Open();

            bookAdapter.Fill(bookTable);
            ratingAdapter.Fill(ratingTable);
            Console.WriteLine(ratingTable.Rows.Count);
            decimal totalRating = 0;
            int ratingCount = 0;

            foreach (DataRow row in ratingTable.Rows)
            {
                var userRating = new UserRatingViewModel
                {
                    Name = row["User_Name"].ToString() ?? "",
                    Rating = Convert.ToDecimal(row["Rating"]),
                    Comment = row["Description"].ToString() ?? ""
                };
                model.UsersRating.Add(userRating);

                totalRating += userRating.Rating;
                ratingCount++;
            }

            if (bookTable.Rows.Count > 0)
            {
                var row = bookTable.Rows[0];
                model.Id = Convert.ToInt32(row["Book_Id"]);
                model.Title = row["Title"].ToString() ?? "";
                model.Author = row["Author_Name"].ToString() ?? "";
                model.Publisher = row["Publisher"].ToString() ?? "Unknown";
                model.Language = row["Language"].ToString() ?? "Unknown";
                model.CoverImage = row["Cover_Image"].ToString() ?? "";
                model.AvgRating = ratingCount > 0 ? totalRating / ratingCount : 0;
            }

            connection.Close();
        }
        Console.WriteLine(new
        {
            model.Id,
            model.Title,
            model.Author
        });
        return View("BookDetails", model);
    }


}
