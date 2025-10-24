namespace Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public float Rate { get; set; }

        public int BookId { get; set; }

        public required Book Book { get; set; }


    }
}