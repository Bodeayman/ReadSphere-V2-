using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages
{
    public class AddGoalsModel : PageModel
    {
        public class newBook
        {
            public string Title { get; set; }
            public int Id { get; set; }
        }
        public List<newBook> bookslist { get; set; }


        [BindProperty]
        public DateTime noti_time { get; set; }
        [BindProperty]
        public int target_pages { get; set; }
        [BindProperty]
        public int BookID { get; set; }
        [BindProperty]
        public string noti_message { get; set; }


        public void OnGet()
        {
            bookslist = new List<newBook>();
            //Beware the syntax

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "select Book_id , title from book where book_id  in (select BookId from booksPossess where ownerid = @owner)";

                Console.WriteLine(Convert.ToInt32(Request.Cookies["user_id"]));
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

                DataTable dataTable = new DataTable();
                try
                {
                    connection.Open();

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int Id = Convert.ToInt32(row["Book_id"]);
                        string title = row["title"].ToString();

                        newBook book = new newBook();
                        book.Id = Id;
                        book.Title = title;

                        bookslist.Add(book);
                    }
                    connection.Close();
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }


        public IActionResult OnPost()
        {
            int selectedBookId = BookID;
            Console.WriteLine(BookID);
            bookslist = bookslist ?? new List<newBook>();

            if (!ModelState.IsValid)
            {


                return Page();
            }
            string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            Random random = new();

            int randomNumber = random.Next(0, 10000);
            string query = "INSERT INTO [reading_goal] (goal_id,user_id,target_pages,book_id) " +
                           "VALUES (@id,@user_id,@target,@book)";

            string query2 = "Insert into [notification] values (@id,@message,@date)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);


                cmd.Parameters.AddWithValue("@user_id", Convert.ToInt32(Request.Cookies["user_id"]));
                cmd.Parameters.AddWithValue("@id", randomNumber);
                cmd.Parameters.AddWithValue("@target", target_pages);
                cmd.Parameters.AddWithValue("@book", BookID);



                SqlCommand cmd2 = new SqlCommand(query2, connection);

                cmd2.Parameters.AddWithValue("@id", randomNumber);
                cmd2.Parameters.AddWithValue("@message", noti_message);
                cmd2.Parameters.AddWithValue("@date", noti_time);



                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return Page();
                }
            }

            return RedirectToPage("/Index");
        }

    }
}
