namespace Models
{

    public class Quote
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public required Book Book { get; set; }

        public string QuoteText { get; set; } = string.Empty;
        public int PageNumber { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}