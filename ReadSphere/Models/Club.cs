namespace Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public List<String> Users { get; set; }

    }

}