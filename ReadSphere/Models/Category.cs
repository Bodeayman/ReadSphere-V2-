namespace Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Unknown";
        public string Description { get; set; } = "Default Description";

        public required ICollection<Book> Books { get; set; }



    }
}