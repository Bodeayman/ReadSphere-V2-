

namespace Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Unknown Title";
        public string Author { get; set; } = "Unknown";
        public string? Publisher { get; internal set; }
        public double AvgRate { get; set; }
        public string? CoverImage { get; internal set; }
        public string? Language { get; internal set; }

        public ICollection<Rating>? UsersRatings { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Note>? Notes { get; set; }
        public ICollection<Quote>? Quotes { get; set; }
        public ICollection<Goal>? Goals { get; set; }
        public ICollection<User>? Users { get; set; }


    }
}