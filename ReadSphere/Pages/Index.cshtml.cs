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
  public List<Quote> myquotes { get; set; }


  public class Book
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }

    public string Publisher { get; set; }
    public string Language { get; set; }

    public string cover_image { get; set; }

    public double avgRate { get; set; }
  }

  public class Club
  {
    public string name { get; set; }
    public string desc { get; set; }

  }

  public class Note
  {
    public string author { get; set; }
    public string desc { get; set; }
  }
  public class Quote
  {
    public string author { get; set; }
    public string quote
    { get; set; }
    public string book { get; set; }
  }
  public void OnGet()
  {
    mybooks = new List<Book>();
    myclubs = new List<Club>();
    myquotes = new List<Quote>();


    string query = " select* from book where book_id  in (select BookId from booksPossess where ownerid = @owner)";
    string query2 = " select* from club where club_id  in (select club_id from clubs_joined where user_id = @owner)";
    string query3 = " select * from quote join book on quote.book_id = book.Book_Id where owner_quote_id = @owner";
    string query4 = "select * from book join review on book.Review_Id = review.Review_Id where Book_Id = @bookID";

    using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
    {

      SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      DataTable dataTable = new DataTable();





      DataTable ratingforbook = new DataTable();

      try
      {
        connection.Open();

        dataAdapter.Fill(dataTable);
        double rating = 0;
        double countRating = 0;
        foreach (DataRow row in dataTable.Rows)
        {
          int Id = Convert.ToInt32(row["Book_Id"]);
          string title = row["Title"].ToString();
          string Author = row["Author_Name"].ToString();
          string publisher = row["Publisher"].ToString();
          string Language = row["Language"].ToString();
          string cover_image = row["Cover_Image"].ToString();
          SqlDataAdapter ratingTable = new SqlDataAdapter(query4, connection);
          ratingTable.SelectCommand.Parameters.AddWithValue("@bookID", Id);

          ratingTable.Fill(ratingforbook);
          foreach (DataRow ratingrow in ratingforbook.Rows)
          {
            rating += Convert.ToInt32(ratingrow["Rating"]);
            countRating++;
          }
          Book book = new Book();
          book.Id = Id;
          book.Title = title;
          book.Author = Author;
          book.Publisher = publisher;
          book.Language = Language;
          book.cover_image = cover_image;
          book.avgRate = rating / countRating;
          mybooks.Add(book);
        }
        Console.WriteLine(mybooks.Count);
      }

      catch (Exception ex)
      {
        Console.WriteLine($"An error Happended: {ex.Message}");
      }

      dataAdapter = new SqlDataAdapter(query2, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();


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


      dataAdapter = new SqlDataAdapter(query3, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();



      dataAdapter.Fill(dataTable);

      foreach (DataRow row in dataTable.Rows)
      {
        string desc = Convert.ToString(row["quote_text"]);
        string name = Convert.ToString(row["Author_Name"]); //
        string book = Convert.ToString(row["Title"]);


        Quote quote = new();
        quote.author = name;
        quote.quote = desc;
        quote.book = book;

        myquotes.Add(quote);
      }
      Console.WriteLine(myclubs.Count);
      connection.Close();
    }

  }
}
