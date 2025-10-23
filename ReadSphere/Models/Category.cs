namespace Models
{
    public class Cat
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }

        public List<string>? books { get; set; }

    }
}