using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class CreateCategoryModel : PageModel
{

    [BindProperty]
    public string Name { get; set; }
    [BindProperty]

    public string Description { get; set; }
    public void OnGet()
    {
    }
}
