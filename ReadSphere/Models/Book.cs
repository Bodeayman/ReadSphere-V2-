

namespace Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Publisher { get; internal set; }
        public double AvgRate { get; set; }
        public string? CoverImage { get; internal set; }
        public string? Language { get; internal set; }

        public List<UserRating> UsersRatings { get; set; }


    }
}