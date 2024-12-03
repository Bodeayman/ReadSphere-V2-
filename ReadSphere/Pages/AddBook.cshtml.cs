using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class AddBookModel : PageModel
{


    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        public string Language { get; set; }

        public string cover_image { get; set; }

        public string review_id { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //No more orm code
        return RedirectToPage("/Index");
    }
}
