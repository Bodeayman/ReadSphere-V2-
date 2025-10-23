using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AddBookModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        public string Publisher { get; set; }
        public string Language { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; }
    }
}