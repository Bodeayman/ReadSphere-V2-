using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AddQuoteViewModel
    {
        [Required(ErrorMessage = "Quote text is required.")]
        [Display(Name = "Quote Text")]
        public string QuoteText { get; set; }

        [Required(ErrorMessage = "Page number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be positive.")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "You must select a book.")]
        [Display(Name = "Select Book")]
        public int BookID { get; set; }

        public List<BookItem> BooksList { get; set; } = new();

        public class BookItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}
