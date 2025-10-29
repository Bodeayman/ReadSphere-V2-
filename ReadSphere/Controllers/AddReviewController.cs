using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using ReadSphere.Data;
using ViewModels;

public class AddReviewController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<User> _userManager;

    public AddReviewController(ApplicationDBContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;

    }
    // GET: /AddReview/AddReview/5
    [HttpGet]
    public async Task<IActionResult> AddReview(int id) // id = BookId
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
            return NotFound();

        var model = new AddReviewViewModel
        {
            BookId = id
        };

        return View("AddReview", model);
    }

    // POST: /AddReview/AddRating
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRating(AddReviewViewModel model)
    {
        try
        {


            if (!ModelState.IsValid)
                return View("AddReview", model);

            var user = await _userManager.GetUserAsync(User);
            var userStringId = user.Id;
            var book = await _context.Books.FindAsync(model.BookId);
            var foundRating = await _context.Ratings.AnyAsync(R => R.UserId == user.Id && R.BookId == book.Id);
            if (foundRating)
            {
                Console.WriteLine("You have this rating before");
                return RedirectToAction("Index", "AllBooks", new { id = model.BookId });

            }
            if (book == null)
                return NotFound();

            var review = new Rating
            {
                User = user,
                UserId = user.Id,
                BookId = model.BookId,
                Book = book,
                Rate = model.Rating,
                Comment = model.ReviewText
            };

            _context.Ratings.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "AllBooks", new { id = model.BookId });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("AddReview", "Index");
        }
    }
}
