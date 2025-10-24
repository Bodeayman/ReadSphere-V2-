using Microsoft.AspNetCore.Authorization;
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
    [Authorize]

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = new BookDetailsViewModel();

        Book RequestedBook = await _context.Books.
        FirstOrDefaultAsync(Book => Book.Id == id) ?? new Book();
        List<Rating> RatingsForBook = await _context.Ratings
        .Where(Rating => Rating.BookId == id).Include(Rating => Rating.User)
        .ToListAsync();

        float totalRating = 0;
        int ratingCount = 0;
        if (RequestedBook == null)
        {
            Console.WriteLine("This book is not found");
            return View("BookDetails", model);
        }
        foreach (Rating Rating in RatingsForBook)
        {
            var userRating = new UserRatingViewModel
            {
                Name = Rating.User.UserName,
                Rating = Rating.Rate,
                Comment = Rating.Comment ?? "Invalid Comment"
            };
            model.UsersRating.Add(userRating);

            totalRating += userRating.Rating;
            ratingCount++;
        }
        float averageRate = CalculateAverageRating(totalRating, ratingCount);
        Console.WriteLine("The average rate of this book is: {0}", averageRate);
        model.Id = RequestedBook.Id;
        model.Title = RequestedBook.Title;
        model.Author = RequestedBook.Author;
        model.Publisher = RequestedBook.Publisher;
        model.Language = RequestedBook.Language;
        model.CoverImage = RequestedBook.CoverImage;
        model.AvgRating = averageRate;


        return View("BookDetails", model);
    }

    private static float CalculateAverageRating(float total, int number)
    {
        if (number > 0)
        {
            return total / number;
        }
        return 0;
    }


}
