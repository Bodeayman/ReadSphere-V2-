using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages
{
    public class CategoriesModel : PageModel
    {
        public List<Cat>? Cats { get; set; }
        public int count { get; set; }
        public int CatId { get; set; }



        public class Cat
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Desc { get; set; }

            public List<string>? books { get; set; }

        }
        public void OnGet()
        {
            Cats = new List<Cat>();


            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "select * from category";
                string query2 = "select  book.Title,CATEGORY.Category_Id from  book_category,book,category where book.book_id = BOOK_CATEGORY.Book_Id and book_category.Category_Id = category.category_id and category.Category_Id = @cat_id";

                SqlCommand cmd = new(query, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Cat category = new Cat();
                        int Id = Convert.ToInt32(row["category_id"]);
                        string Name = row["category_name"].ToString() ?? "Unknown";
                        string Desc = row["category_desc"].ToString() ?? "Unknown";
                        category.Id = Id;
                        category.Name = Name;
                        category.Desc = Desc;
                        category.books = new List<String>();
                        SqlDataAdapter cats_book_adapter = new SqlDataAdapter(query2, connection);


                        DataTable cats_book_table = new DataTable();

                        cats_book_adapter.SelectCommand.Parameters.AddWithValue("@cat_id", Id);

                        cats_book_adapter.Fill(cats_book_table);

                        foreach (DataRow book in cats_book_table.Rows)
                        {
                            category.books.Add(Convert.ToString(book["Title"]));
                        }




                        Cats.Add(category);
                    }
                    count = Cats.Count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        public IActionResult OnPost(int CatId)
        {
            return RedirectToPage("AddBookCat", new { CatId = CatId });
        }
    }
}
