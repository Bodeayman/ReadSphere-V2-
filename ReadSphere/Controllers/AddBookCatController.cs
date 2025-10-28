using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadSphere.Data;
using ViewModels;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadSphere.Controllers
{
    public class AddBookCatController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AddBookCatController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookCatPage(int id)
        {
            var model = new AddBookCatViewModel
            {
                CategoryID = id,
                BooksList = await _context.Books
                    .Select(b => new BookItem
                    {
                        Id = b.Id,
                        Title = b.Title
                    })
                    .ToListAsync()
            };

            return View("AddBookCat", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBookToCategory(AddBookCatViewModel model)
        {
            try
            {
                // Ensure valid model
                if (!ModelState.IsValid)
                    return View("AddBookCat", model);

                // Retrieve entities
                var book = await _context.Books.FindAsync(model.BookID);
                var category = await _context.Categories
                    .Include(c => c.Books)
                    .FirstOrDefaultAsync(c => c.Id == model.CategoryID);

                if (book == null || category == null)
                    return NotFound();

                // Avoid duplicate entries
                if (category.Books.Any(b => b.Id == book.Id))
                    return RedirectToAction("Index", "Home");

                // Add relation
                category.Books.Add(book);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book to category: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
