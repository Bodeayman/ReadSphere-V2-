using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadSphere.Data;
using Models;
using ViewModels;

namespace ReadSphere.Controllers
{
    [Authorize]
    public class AddQuoteController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;

        public AddQuoteController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AddQuotePage()
        {
            var user = await _userManager.GetUserAsync(User);

            // Get all books owned by the user
            var books = await _context.Books
                                .Where(b => b.Users.Any(u => u.Id == user.Id))
                                .ToListAsync();

            // Map to view model
            var model = new AddQuoteViewModel
            {
                BooksList = books.Select(b => new AddQuoteViewModel.BookItem
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuote(AddQuoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await AddQuotePage(); // reuse GET to reload books
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var Book = await _context.Books.FirstOrDefaultAsync(B => B.Id == model.BookID);
                var quote = new Quote
                {
                    Book = Book,
                    QuoteText = model.QuoteText,
                    CreatedAt = DateTime.Now,
                    PageNumber = model.PageNumber,
                    BookId = model.BookID,
                    UserId = user.Id
                };

                _context.Quotes.Add(quote);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Quote added successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to add quote.");
                return await AddQuotePage();
            }
        }

        // Optional: Get all quotes related to the user
        private async Task<List<Quote>> GetUserQuotes(string userId)
        {
            return await _context.Quotes
                        .Include(q => q.Book)
                        .Where(q => q.UserId == userId)
                        .ToListAsync();
        }
    }
}
