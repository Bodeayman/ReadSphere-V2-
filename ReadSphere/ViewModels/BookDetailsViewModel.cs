using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
    public class UserRatingViewModel
    {
        public string Name { get; set; } = "";
        public float Rating { get; set; }
        public string Comment { get; set; } = "";
    }

    public class BookDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string Language { get; set; } = "";
        public string CoverImage { get; set; } = "";
        public float AvgRating { get; set; }

        public List<UserRatingViewModel> UsersRating { get; set; } = new List<UserRatingViewModel>();

        // For binding user input
        public string? Comment { get; set; }
        public float? Rating { get; set; }
    }
}
