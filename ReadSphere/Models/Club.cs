namespace Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Unknown Name";
        public string Description { get; set; } = "Default Description";

        public required ICollection<User> Users { get; set; }

    }

}