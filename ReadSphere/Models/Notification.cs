namespace Models
{
    public class Notification
    {

        public int Id { get; set; }
        public string Message { get; set; } = "Remember reading this book";
        public DateTime DueDate { get; set; }
        public int Pages { get; set; }

        public int GoalId { get; set; }
        public required Goal Goal { get; set; }


    }
}