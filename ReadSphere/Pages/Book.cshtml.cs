using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

public class BookModel : PageModel
{
    public List<Book> Books { get; set; }

    public void OnGet()
    {
        // Mock data (Replace with data from your database)
        Books = new List<Book>
        {
            new Book { Id = 1, Title = "The Alchemist", Author = "Ayman" },
            new Book { Id = 2, Title = "1984", Author = "Dsoqi" },
            new Book { Id = 3, Title = "To Kill a Mockingbird", Author = "somebody" }
        };
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
