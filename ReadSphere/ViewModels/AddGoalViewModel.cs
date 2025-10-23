using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AddGoalViewModel
    {
        public class BookOption
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        // Dropdown list for user’s books
        public List<BookOption> BooksList { get; set; } = new List<BookOption>();

        [Required(ErrorMessage = "Please enter a target number of pages.")]
        [Range(1, int.MaxValue, ErrorMessage = "Target pages must be positive.")]
        public int TargetPages { get; set; }

        [Required(ErrorMessage = "Please select a book.")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Please enter a notification message.")]
        public string NotiMessage { get; set; }

        [Required(ErrorMessage = "Please select a notification date/time.")]
        public DateTime NotiTime { get; set; }
    }
}
