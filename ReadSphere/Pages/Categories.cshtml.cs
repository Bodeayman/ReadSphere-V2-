using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReadSphere.Pages
{
    public class CategoriesModel : PageModel
    {
        public List<Cat> Cats { get; set; }
        public int count { get; set; }


        public class Cat
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }


        }
        public void OnGet()
        {
            Cats = new List<Cat>();

            using (SqlConnection connection = new SqlConnection("Server=ENGABDULLAH;Database=ReadSphere;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"))
            {
                string query = "select * from category";

                SqlCommand cmd = new(query, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int Id = Convert.ToInt32(row["category_id"]);
                        string Name = row["category_name"].ToString() ?? "Unknown";
                        string Desc = row["category_desc"].ToString() ?? "Unknown";


                        Cat category = new Cat();
                        category.Id = Id;
                        category.Name = Name;
                        category.Desc = Desc;
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
    }
}
