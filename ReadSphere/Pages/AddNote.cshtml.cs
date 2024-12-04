using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class AddNoteModel : PageModel
{
    [BindProperty]
    public string note_text { get; set; }
    [BindProperty]
    public int page_number { get; set; }
    public void OnGet()
    {
    }
}
