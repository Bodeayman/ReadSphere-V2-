using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        [Display(Name = "Category Description")]
        public string Description { get; set; }
    }
}
