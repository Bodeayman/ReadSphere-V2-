using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


public class AddQuoteModel : PageModel
{
    public class newBook
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }
    public List<newBook> bookslist { get; set; }


    [BindProperty]
    public string quote_text { get; set; }
    [BindProperty]
    public int page_number { get; set; }
    [BindProperty]
    public int BookID { get; set; }
    public void OnGet()
    {
        bookslist = new List<newBook>();

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
        string query = "INSERT INTO Quote (Quote_Id,Book_Id,quote_text,Added_date,Page_number,owner_quote_id) " +
                       "VALUES (@id,@book_id,@quote_text,@added_date,@page,@owner)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);


            cmd.Parameters.AddWithValue("@book_id", 1);
            cmd.Parameters.AddWithValue("@id", randomNumber);
            cmd.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));
            cmd.Parameters.AddWithValue("@quote_text", quote_text);
            cmd.Parameters.AddWithValue("@added_date", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@page", page_number);


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
