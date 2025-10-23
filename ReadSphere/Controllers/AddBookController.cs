using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO;
using ViewModels;


namespace Controllers
{

    public class AddBookController : Controller
    {
        [HttpGet]
        public IActionResult AddBookPage()
        {
            Console.WriteLine("Accessing");
            return View(new AddBookModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBookToDB(AddBookModel model)
        {
            /*string Title, string Author, string Publisher, string Language, IFormFile cover_image*/
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title) || model.CoverImage == null)
                {
                    ModelState.AddModelError("", "Title and cover image are required.");
                    return View("AddBookPage", model);
                }
                Console.WriteLine("The file is working good here, i guesss");


                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Path.GetFileName(model.CoverImage.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    model.CoverImage.CopyTo(fileStream);

                string imageUrl = "uploads/" + fileName;
                string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

                int randomId = new Random().Next(0, 10000);
                string query = @"INSERT INTO BOOK (Book_Id, Title, Author_Name, Publisher, Language, Cover_Image)
                         VALUES (@id, @Title, @Author, @Publisher, @Language, @CoverImage)";

                using (SqlConnection connection = new(connectionString))
                using (SqlCommand cmd = new(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", randomId);
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Author", model.Author);
                    cmd.Parameters.AddWithValue("@Publisher", model.Publisher);
                    cmd.Parameters.AddWithValue("@Language", model.Language);
                    cmd.Parameters.AddWithValue("@CoverImage", imageUrl);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        ModelState.AddModelError("", "Error adding book.");
                        return View("AddBookPage");
                    }
                }
                Console.WriteLine("The file is working finllay , and the operation is saved");
                return View("AddBookPage", model);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("AddBookPage");

            }
        }

        // Example index action

    }

}