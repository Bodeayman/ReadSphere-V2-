using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class AddBookModel : PageModel
{
    private readonly DataContext _context;

    public AddBookModel(DataContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Book Book { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Books.Add(Book);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}
