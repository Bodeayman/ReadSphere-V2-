using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class AddQuoteModel : PageModel
{
    [BindProperty]
    public string quote_text { get; set; }
    [BindProperty]
    public int page_number { get; set; }
    public void OnGet()
    {
    }
}
