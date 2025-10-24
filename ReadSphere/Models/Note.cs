namespace Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string NoteText { get; set; }

        public DateTime DateTime { get; set; }

        public int BookId { get; set; }
        public required Book Book { get; set; }
        public string UserId { get; set; }   // FK
        public User User { get; set; }

        public int PageNumber { get; set; }
    }

}