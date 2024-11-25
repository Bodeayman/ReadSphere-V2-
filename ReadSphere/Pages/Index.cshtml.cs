using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReadSphere.Pages;

public class IndexModel : PageModel
{
  private readonly ILogger<IndexModel> _logger;

  public IndexModel(ILogger<IndexModel> logger)
  {
    _logger = logger;
  }

  //The attributes of the model
  public string Name { get; set; }
  public int Age { get; set; }


  //Onget from the redirect of the Login page will take from the object in cshtml
  public void OnGet(string name, int age)
  {
    Name = name;
    Age = age;
  }
}
