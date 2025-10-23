

namespace Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Publisher { get; internal set; }
        public double avgRate { get; set; }
        public string? cover_image { get; internal set; }
        public string? Language { get; internal set; }

        public List<User_Rating> Users_rating { get; set; }


    }
}