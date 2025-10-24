using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using ReadSphere.Data;
using System.Data;
using ViewModels;

public class BookDetailsController : Controller
{

    private readonly ApplicationDBContext _context;
    public BookDetailsController(ApplicationDBContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        Console.WriteLine("Working on the details page here");
        var model = new BookDetailsViewModel();
        Console.WriteLine("ID is {0}", id);
        Book RequestedBook = await _context.Books.FirstOrDefaultAsync(Book => Book.Id == id);
        List<Rating> RatingsForBook = await _context.UserRating
        .Where(Rating => Rating.BookId == id)

        .ToListAsync();

        decimal totalRating = 0;
        int ratingCount = 0;

        foreach (Rating Rating in RatingsForBook)
        {
            var userRating = new UserRatingViewModel
            {
                Name = row["User_Name"].ToString() ?? "",
                Rating = Rating.Rate,
                Comment = Rating.Comment
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

        Console.WriteLine(new
        {
            model.Id,
            model.Title,
            model.Author
        });
        return View("BookDetails", model);
    }


}
