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
        public string noti_time { get; set; }
        [BindProperty]
        public int target_pages { get; set; }
        [BindProperty]
        public int BookID { get; set; }


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
            bookslist = bookslist ?? new List<newBook>();

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Working");


                return Page();
            }
            string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            Random random = new();

            int randomNumber = random.Next(0, 10000);
            string query = "INSERT INTO [Daily_Goal] (goal_id,book_id,target_pages,notification_time) " +
                           "VALUES (@id,@book_id,@target,@time)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);


                cmd.Parameters.AddWithValue("@book_id", BookID);
                cmd.Parameters.AddWithValue("@id", randomNumber);

                cmd.Parameters.AddWithValue("@target", target_pages);
                cmd.Parameters.AddWithValue("@added_date", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@time", noti_time);
                cmd.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));



                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
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
