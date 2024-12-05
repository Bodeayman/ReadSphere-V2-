using System.Data;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReadSphere.Pages;


public class AddNoteModel : PageModel
{
    public class newBook
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }
    public List<newBook> bookslist { get; set; }


    [BindProperty]
    public string note_text { get; set; }
    [BindProperty]
    public int page_number { get; set; }
    [BindProperty]
    public int BookID { get; set; }


    public void OnGet()
    {
        bookslist = new List<newBook>();
        //Beware the syntax

        using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
        {
            // SQL query to insert a worker into the Workers table
            string query = "select Book_id , title from book where book_id  in (select BookId from booksPossess where ownerid = @owner)";

            Console.WriteLine(Convert.ToInt32(Request.Cookies["user_id"]));
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

            DataTable dataTable = new DataTable();
            try
            {
                // Open the connection
                connection.Open();

                // Fill the DataTable with the data from the database
                dataAdapter.Fill(dataTable);

                // Process the data from the DataTable
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

        // SQL query to insert a new book into the BOOK table
        Random random = new();

        int randomNumber = random.Next(0, 10000);
        string query = "INSERT INTO Note (Note_Id,Book_Id,Note_Text,Added_date,Page_number,owner_note_id) " +
                       "VALUES (@id,@book_id,@note_text,@added_date,@page,@owner)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, connection);

            // parameters to avoid SQL injection
            // We need to put the id of the real book here but after adding the quotes
            // and the notes
            cmd.Parameters.AddWithValue("@book_id", BookID);
            cmd.Parameters.AddWithValue("@id", randomNumber);

            cmd.Parameters.AddWithValue("@note_text", note_text);
            cmd.Parameters.AddWithValue("@added_date", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@page", page_number);
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

        return RedirectToPage("/Index");// redirection to books page
    }

}
