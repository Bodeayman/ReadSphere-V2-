using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using ReadSphere.Data;
using System;
using System.Collections.Generic;
using System.Data;
using ViewModels;

namespace Controllers
{
    public class AllBooksController : Controller
    {


        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;

        // GET: /Books
        public AllBooksController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new BookViewModel
            {
                Books = await GetAllBooks()
            };
            viewModel.Count = viewModel.Books.Count;
            return View("AllBooks", viewModel);
        }

        // POST: /Books/AddToUser
        [HttpPost]
        public async Task<IActionResult> AddToUser(int id)
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            bool success = await AddBookToUser(user.Id, id);

            if (success)
            {
                Console.WriteLine("Returning Right now");
                return RedirectToAction("Index");
            }


            TempData["ErrorMessage"] = "Failed to add book.";
            return RedirectToAction("Index");
        }

        private async Task<List<Book>> GetAllBooks()
        {

            var Books = await _context.Books.ToListAsync();
            foreach (var Book in Books)
            {
                var book = new Book
                {
                    Id = Book.Id,
                    Title = Book.Title,
                    Author = Book.Author,
                    Publisher = Book.Publisher,
                    Language = Book.Language,
                    CoverImage = Book.CoverImage,
                    AvgRate = Book.AvgRate,
                    UsersRatings = new List<Rating>()
                };

            }
            return Books;

        }

        private async Task<bool> AddBookToUser(string userId, int bookId)
        {
            bool success = false;
            {
                string checkBookExistsQuery = "SELECT COUNT(*) FROM BOOK WHERE Book_Id = @BookId";
                string insertQuery = "INSERT INTO BooksPossess (OwnerId, BookId) VALUES (@UserId, @BookId)";

                try
                {
                    var user = await _userManager.FindByIdAsync(userId);

                    var book = await _context.Books.FindAsync(bookId);
                    _context.Entry(user).Collection(u => u.Books).Load();

                    user.Books.Add(book);
                    await _context.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding book to user: {ex.Message}");
                }
            }
            return success;
        }
    }
}
