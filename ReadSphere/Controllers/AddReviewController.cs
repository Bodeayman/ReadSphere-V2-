using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModels;

public class AddReviewController : Controller
{
    private readonly string _connectionString =
        "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

    // GET: /BookDetails/AddReview/5
    [HttpGet]
    public IActionResult AddReview(int id) // <-- Book ID passed from URL
    {
        if (Request.Cookies["User"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new AddReviewModel
        {
            BookId = id // assign the book ID to the model
        };

        return View(model); // load the AddReview.cshtml view with the book info
    }

    // POST: /BookDetails/AddRating
    [HttpPost]
    public IActionResult AddRating(AddReviewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("AddReview", model);
        }

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Review (ReviewId, BookId, Rating, ReviewText, OwnerId, AddedDate)
                                 VALUES (@id, @bookId, @rating, @review_text, @owner, @date)";

                SqlCommand cmd = new SqlCommand(query, connection);

                Random rnd = new();
                int randomId = rnd.Next(10000, 99999);

                cmd.Parameters.AddWithValue("@id", randomId);
                cmd.Parameters.AddWithValue("@bookId", model.BookId);
                cmd.Parameters.AddWithValue("@rating", model.Rating);
                cmd.Parameters.AddWithValue("@review_text", model.Review_Text);
                cmd.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));
                cmd.Parameters.AddWithValue("@date", DateTime.Now);

                connection.Open();
                cmd.ExecuteNonQuery();
            }

            // After adding the review, go back to that book's detail page
            return RedirectToAction("Details", new { id = model.BookId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return View("AddReview", model);
        }
    }
}
