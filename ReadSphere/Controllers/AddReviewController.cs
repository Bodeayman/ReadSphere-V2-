using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        if (!ModelState.IsValid)
            return View("AddReview", model);

        var book = await _context.Books.FindAsync(model.BookId);
        var user = await _userManager.GetUserAsync(User);
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

        return RedirectToAction("Details", "AllBooks", new { id = model.BookId });
    }
}
