using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReadSphere.Pages
{
    public class ClubsModel : PageModel
    {
        public List<String> clubs = ["Zewail City Club", "ICPC Cooperation", "IEEE of ZC"];
        public void OnGet()
        {


        }
    }
}
