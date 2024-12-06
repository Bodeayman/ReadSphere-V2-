using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages;

public class IndexModel : PageModel
{
  private readonly ILogger<IndexModel> _logger;

  public IndexModel(ILogger<IndexModel> logger)
  {
    _logger = logger;
  }
  public List<Book> mybooks { get; set; }
  public List<Club> myclubs { get; set; }

  public class Book
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }

    public string Publisher { get; set; }
    public string Language { get; set; }

    public string cover_image { get; set; }

    public string review_id { get; set; }
  }

  public class Club
  {
    public string name { get; set; }
    public string desc { get; set; }

  }
  public void OnGet()
  {
    mybooks = new List<Book>();
    myclubs = new List<Club>();

    string query = " select* from book where book_id  in (select BookId from booksPossess where ownerid = @owner)";
    string query2 = " select* from club where club_id  in (select club_id from clubs_joined where user_id = @owner)";

    using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
    {

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
          int Id = Convert.ToInt32(row["Book_Id"]);
          string title = row["Title"].ToString();
          string Author = row["Author_Name"].ToString();
          string publisher = row["Publisher"].ToString();
          string Language = row["Language"].ToString();
          string cover_image = row["Cover_Image"].ToString();
          string review_id = row["Review_Id"].ToString();
          Book book = new Book();
          book.Id = Id;
          book.Title = title;
          book.Author = Author;
          book.Publisher = publisher;
          book.Language = Language;
          book.cover_image = cover_image;
          book.review_id = review_id;
          mybooks.Add(book);
        }
        Console.WriteLine(mybooks.Count);
        connection.Close();
      }

      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }

      dataAdapter = new SqlDataAdapter(query2, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();

      try
      {
        connection.Open();

        dataAdapter.Fill(dataTable);

        foreach (DataRow row in dataTable.Rows)
        {
          string desc = Convert.ToString(row["club_description"]);
          string name = Convert.ToString(row["club_name"]); //

          Club club = new();
          club.desc = desc;
          club.name = name;

          myclubs.Add(club);
        }
        Console.WriteLine(myclubs.Count);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }

    }

  }
}
