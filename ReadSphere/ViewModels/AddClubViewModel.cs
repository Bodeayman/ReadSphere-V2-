using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AddClubViewModel
    {
        [Required(ErrorMessage = "Club name is required.")]
        [Display(Name = "Club Name")]
        public string ClubName { get; set; }

        [Required(ErrorMessage = "Club description is required.")]
        [Display(Name = "Description")]
        public string ClubDescription { get; set; }
    }
}
