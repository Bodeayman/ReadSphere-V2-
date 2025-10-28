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
    public class AddNoteController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;

        public AddNoteController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AddNotePage()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new AddNoteViewModel
            {
                BooksList = await _context.Books
                                    .Where(b => b.Users.Any(u => u.Id == user.Id))
                                    .Select(b => new AddNoteViewModel.BookOption
                                    {
                                        Id = b.Id,
                                        Title = b.Title
                                    })
                                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(AddNoteViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model.BooksList = await _context.Books
                                        .Where(b => b.Users.Any(u => u.Id == user.Id))
                                        .Select(b => new AddNoteViewModel.BookOption
                                        {
                                            Id = b.Id,
                                            Title = b.Title
                                        })
                                        .ToListAsync();
                return View("AddNotePage", model);
            }

            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.BookID);

                var note = new Note
                {
                    BookId = model.BookID,
                    Book = book,
                    NoteText = model.NoteText,
                    PageNumber = model.PageNumber,
                    DateTime = DateTime.Now,
                    User = user,
                    Author = "",
                    UserId = user.Id
                };

                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Note added successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to add note.");

                model.BooksList = await _context.Books
                                        .Where(b => b.Users.Any(u => u.Id == user.Id))
                                        .Select(b => new AddNoteViewModel.BookOption
                                        {
                                            Id = b.Id,
                                            Title = b.Title
                                        })
                                        .ToListAsync();
                return View("AddNotePage", model);
            }
        }

        // Optional: Get all notes related to the user
        private async Task<List<Note>> GetUserNotes(string userId)
        {
            return await _context.Notes
                        .Include(n => n.Book)
                        .Where(n => n.UserId == userId)
                        .ToListAsync();
        }
    }
}
