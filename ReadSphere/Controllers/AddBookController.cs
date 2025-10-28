using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ReadSphere.Data;
using System.IO;
using ViewModels;


namespace Controllers
{

    public class AddBookController : Controller
    {

        private readonly ApplicationDBContext _context;
        public AddBookController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddBookPage()
        {
            Console.WriteLine("Accessing");
            return View(new AddBookModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookToDB(AddBookModel model)
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
                try
                {
                    var Book = await _context.Books.AddAsync(new Models.Book
                    {
                        Author = model.Author,
                        Title = model.Title,
                        Publisher = model.Publisher,
                        Language = model.Language,
                        CoverImage = imageUrl,
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "Error adding book.");
                    return View("AddBookPage");
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


    }

}