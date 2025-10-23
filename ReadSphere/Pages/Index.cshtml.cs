using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using Models;
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
  public List<Note> mynotes { get; set; }

  [BindProperty]
  public string SearchQuery { get; set; }







  public int TotalBooks { get; set; }
  public int TotalClubs { get; set; }
  public int TotalQuotes { get; set; }
  public int TotalNotes { get; set; }
  public int TotalUsers { get; set; }
  string connectionString = "Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
  public List<Notification> ongoing { get; set; }

  public List<Notification> duegoing { get; set; }

  private int GetCount(string query, string connectionString)
  {
    int count = 0;
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
      try
      {
        SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));
        DataTable dataTable = new DataTable();
        connection.Open();
        dataAdapter.Fill(dataTable);


        if (dataTable.Rows.Count > 0)
        {
          count = Convert.ToInt32(dataTable.Rows[0][0]);
        }
        connection.Close();
      }
      catch (Exception ex)
      {
        _logger.LogError("An error occurred while retrieving the count: " + ex.Message);
      }
    }
    return count;
  }
  public IActionResult OnPost()
  {
    Response.Cookies.Delete("user_id");
    Response.Cookies.Delete("User");
    Response.Cookies.Delete("is_admin");
    return RedirectToPage("/Index");

  }
  public void OnGet(string SearchQuery)
  {


    TotalBooks = GetCount("SELECT COUNT(*) FROM book ", connectionString);

    TotalClubs = GetCount("SELECT COUNT(*) FROM club ", connectionString);

    TotalQuotes = GetCount("SELECT COUNT(*) FROM quote ", connectionString);

    TotalNotes = GetCount("SELECT COUNT(*) FROM note ", connectionString);

    TotalUsers = GetCount("SELECT COUNT(*) FROM [User]", connectionString);

    Console.WriteLine($"SearchQuery: {SearchQuery}");
    mybooks = new List<Book>();
    myclubs = new List<Club>();
    myquotes = new List<Quote>();
    mynotes = new List<Note>();
    ongoing = new List<Notification>();
    duegoing = new List<Notification>();
    DateTime todayDate = DateTime.Now;

    string query = " select* from book where book_id  in (select BookId from booksPossess where ownerid = @owner)";
    string query2 = " select* from club where club_id  in (select club_id from clubs_joined where user_id = @owner)";
    string query3 = " select * from quote join book on quote.book_id = book.Book_Id where owner_quote_id = @owner";
    string query4 = "select * from book ,book_review,review where book.book_id = book_review.book_id and review.Review_Id = book_review.review_id and  book.Book_Id = @bookID";
    string query5 = " select * from note join book on note.book_id = book.Book_Id where owner_note_id = @owner";
    string query6 = "select * from notification , reading_goal ,book where notification.goal_id = reading_goal.goal_id and book.Book_Id = reading_goal.book_id and reading_goal.user_id = @owner ";

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

        foreach (DataRow row in dataTable.Rows)
        {
          double rating = 0;
          double countRating = 0;
          int Id = Convert.ToInt32(row["Book_Id"]);
          string title = row["Title"].ToString();
          string Author = row["Author_Name"].ToString();
          string publisher = row["Publisher"].ToString();
          string Language = row["Language"].ToString();
          string cover_image = row["Cover_Image"].ToString();
          Console.WriteLine(SearchQuery);

          if (!string.IsNullOrEmpty(SearchQuery))
          {
            Regex regex = new Regex(SearchQuery, RegexOptions.IgnoreCase);

            // Only add the book if it matches the regex for title or author
            if (!regex.IsMatch(title) && !regex.IsMatch(Author))
            {
              continue; // Skip this book if it doesn't match
            }
          }


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
          book.avgRate = countRating != 0 ? rating / countRating : 0;
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
        string? desc = Convert.ToString(row["club_description"]);
        string? name = Convert.ToString(row["club_name"]); //

        Club club = new Club();
        club.Desc = desc;
        club.Name = name;
        club.users = new List<String>();

        myclubs.Add(club);
      }
      Console.WriteLine(myclubs.Count);


      dataAdapter = new SqlDataAdapter(query3, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();



      dataAdapter.Fill(dataTable);

      foreach (DataRow row in dataTable.Rows)
      {
        string? desc = Convert.ToString(row["quote_text"]);
        string? name = Convert.ToString(row["Author_Name"]);
        string? book = Convert.ToString(row["Title"]);


        Quote quote = new();
        quote.author = name;
        quote.quote = desc;
        quote.book = book;
        quote.numberofpages = Convert.ToInt32(row["page_number"]);
        quote.dateTime = Convert.ToDateTime(row["added_date"]);
        myquotes.Add(quote);
      }
      Console.WriteLine(myclubs.Count);


      ///////// This is for the notes section
      dataAdapter = new SqlDataAdapter(query5, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();



      dataAdapter.Fill(dataTable);

      foreach (DataRow row in dataTable.Rows)
      {
        string? desc = Convert.ToString(row["note_text"]);
        string? name = Convert.ToString(row["Author_Name"]);
        string? bookTitle = Convert.ToString(row["Title"]);
        DateTime date = Convert.ToDateTime(row["added_date"]);



        Note note = new Note();
        note.desc = desc;
        note.author = name;
        note.book = bookTitle;
        note.dateTime = date;
        note.numberofpages = Convert.ToInt32(row["page_number"]);

        mynotes.Add(note);
      }
      Console.WriteLine(mynotes.Count);
      //////////////////////

      dataAdapter = new SqlDataAdapter(query6, connection);
      dataAdapter.SelectCommand.Parameters.AddWithValue("@owner", Convert.ToInt32(Request.Cookies["user_id"]));

      dataTable = new DataTable();
      dataAdapter.Fill(dataTable);
      foreach (DataRow row in dataTable.Rows)
      {
        DateTime? timing = Convert.ToDateTime(row["notification_time"]);
        string? message = Convert.ToString(row["notification_message"]);
        string? book_title = Convert.ToString(row["Title"]);
        int number_pages = Convert.ToInt32(row["target_pages"]);


        Notification notification = new();
        notification.time = (DateTime)timing;
        notification.Message = message;
        notification.Title = book_title;
        notification.pages = number_pages;

        Console.WriteLine(todayDate > timing);

        if (todayDate > timing)
        {
          Console.WriteLine(todayDate > timing);
          duegoing.Add(notification);

        }
      }
      connection.Close();
    }

  }
}
