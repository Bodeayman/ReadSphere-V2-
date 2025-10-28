using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadSphere.Data;
using ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadSphere.Controllers
{
    public class AllCategoriesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AllCategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch all categories with related books
                var categories = await _context.Categories
                    .Include(c => c.Books)
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Desc = c.Description,
                        Books = c.Books.Select(b => b.Title).ToList()
                    })
                    .ToListAsync();

                return View("AllCategories", categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult RedirectToAddBook(int catId)
        {
            return RedirectToAction("AddBookCat", "Books", new { CatId = catId });
        }
    }
}
