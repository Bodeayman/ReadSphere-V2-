using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere
{
    public class AddReviewModel : PageModel
    {
        public class newBook
        {
            public string Title { get; set; }
            public int Id { get; set; }
        }
        public List<newBook> bookslist { get; set; }


        [BindProperty]
        public string review_text { get; set; }
        [BindProperty]
        public int rating { get; set; }
        [BindProperty]
        public int BookID { get; set; }
        public int ItemId { get; set; }

        public void OnGet(int itemid)
        {
            ItemId = itemid;
        }
        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {

                return Page();
            }
            string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            // SQL query to insert a new book into the BOOK table
            Random random = new();

            int randomNumber = random.Next(0, 10000);
            string query = "INSERT INTO Review (User_Id,Review_Id,Rating,Description,Creation_Date) " +
                           "VALUES (@owner,@id,@rate,@review_text,@added_date)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                // parameters to avoid SQL injection
                // We need to put the id of the real book here but after adding the quotes
                // and the notes
                cmd.Parameters.AddWithValue("@id", randomNumber);
                cmd.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));
                cmd.Parameters.AddWithValue("@review_text", review_text);
                cmd.Parameters.AddWithValue("@added_date", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@rate", rating);


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

            return RedirectToPage("/Index");// redirection to books page
        }

    }
}