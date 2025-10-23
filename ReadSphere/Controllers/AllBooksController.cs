using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using ViewModels;

namespace Controllers
{
    public class AllBooksController : Controller
    {
        private readonly string _connectionString =
            "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // GET: /Books
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new BookViewModel
            {
                Books = GetAllBooks()
            };
            viewModel.Count = viewModel.Books.Count;
            return View("AllBooks", viewModel);
        }

        // POST: /Books/AddToUser
        [HttpPost]
        public IActionResult AddToUser(int bookId)
        {
            Console.WriteLine($"Received BookId: {bookId}");
            string userId = Request.Cookies["user_id"];

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            bool success = AddBookToUser(Convert.ToInt32(userId), bookId);

            if (success)
            {
                Console.WriteLine("Returning Right now");
                return RedirectToAction("Index");
            }


            TempData["ErrorMessage"] = "Failed to add book.";
            return RedirectToAction("Index");
        }

        private List<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BOOK";
                SqlCommand cmd = new(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var book = new Book
                        {
                            Id = Convert.ToInt32(row["Book_Id"]),
                            Title = row["Title"].ToString(),
                            Author = row["Author_Name"].ToString(),
                            Publisher = row["Publisher"].ToString(),
                            Language = row["Language"].ToString(),
                            cover_image = row["Cover_Image"].ToString(),
                            avgRate = 0,
                            Users_rating = new List<User_Rating>()
                        };
                        books.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading books: {ex.Message}");
                }
            }
            return books;
        }

        private bool AddBookToUser(int userId, int bookId)
        {
            bool success = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string checkBookExistsQuery = "SELECT COUNT(*) FROM BOOK WHERE Book_Id = @BookId";
                string insertQuery = "INSERT INTO BooksPossess (OwnerId, BookId) VALUES (@UserId, @BookId)";

                try
                {
                    connection.Open();

                    SqlCommand checkCmd = new(checkBookExistsQuery, connection);
                    checkCmd.Parameters.AddWithValue("@BookId", bookId);
                    int bookCount = (int)checkCmd.ExecuteScalar();

                    if (bookCount > 0)
                    {
                        SqlCommand insertCmd = new(insertQuery, connection);
                        insertCmd.Parameters.AddWithValue("@UserId", userId);
                        insertCmd.Parameters.AddWithValue("@BookId", bookId);
                        insertCmd.ExecuteNonQuery();
                        success = true;
                    }
                    else
                    {
                        Console.WriteLine("The specified book does not exist.");
                    }
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
