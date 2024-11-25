using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Book model definition
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public List<string> Reviews { get; set; }
    public List<string> Quotes { get; set; }
}

// Repository interface
public interface IBookRepository
{
    Task<Book> GetBookByIdAsync(int id);
}

public class BookRepository : IBookRepository
{
    private readonly List<Book> _books = new List<Book>
    {
        new Book
        {
            Id = 1,
            Title = "The Alchemist",
            Author = "Ayman",
            Description = "A philosophical story about following your dreams.",
            Reviews = new List<string> { "Inspiring!", "A must-read for dreamers." },
            Quotes = new List<string> { "And, when you want something, all the universe conspires in helping you to achieve it." }
        },
        new Book
        {
            Id = 2,
            Title = "1984",
            Author = "Dsoqi",
            Description = "A dystopian novel about totalitarianism.",
            Reviews = new List<string> { "Chilling and thought-provoking.", "A timeless classic." },
            Quotes = new List<string> { "Big Brother is watching you." }
        }
        // Add more books as needed
    };

    public Task<Book> GetBookByIdAsync(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(book);
    }
}

// Page model for book details
public class BookDetailsModel : PageModel
{
    private readonly IBookRepository _bookRepository;

    public Book Book { get; private set; } // Ensure this is defined only once

    public BookDetailsModel(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task OnGetAsync(int id)
    {
        Book = await _bookRepository.GetBookByIdAsync(id);
    }
}