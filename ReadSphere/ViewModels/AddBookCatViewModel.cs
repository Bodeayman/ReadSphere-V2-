namespace ViewModels
{
    public class BookItem
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }

    public class AddBookCatViewModel
    {
        public List<BookItem> BooksList { get; set; } = new();
        public int BookID { get; set; }
        public int CategoryID { get; set; }
    }
}
