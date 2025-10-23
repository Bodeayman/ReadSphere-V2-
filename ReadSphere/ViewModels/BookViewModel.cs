using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class BookViewModel
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public int Count { get; set; }
        public int SelectedBookId { get; set; }
    }
}
