using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AddNoteViewModel
    {
        [Required(ErrorMessage = "Note text is required.")]
        [Display(Name = "Note Text")]
        public string NoteText { get; set; }

        [Required(ErrorMessage = "Page number is required.")]
        [Display(Name = "Page Number")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "You must select a book.")]
        [Display(Name = "Book")]
        public int BookID { get; set; }

        public List<BookOption> BooksList { get; set; } = new();

        public class BookOption
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}
