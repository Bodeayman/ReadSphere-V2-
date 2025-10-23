namespace Models
{

    public class Quote
    {
        public string Author { get; set; }
        public string QuoteText
        { get; set; }
        public DateTime DateTime { get; set; }
        public string Book { get; set; }
        public int NumberOfPages { get; set; }

    }
}