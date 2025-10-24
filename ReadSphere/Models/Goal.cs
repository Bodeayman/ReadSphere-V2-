
namespace Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int TargetPages { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }

        public string UserId { get; set; }   // FK
        public User User { get; set; }
    }
}