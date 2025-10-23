namespace Models
{
    public class Note
    {
        public string Author { get; set; }
        public string Desc { get; set; }

        public DateTime DateTime { get; set; }
        public string Book { get; set; }

        public int NumberOfPages { get; set; }
    }

}