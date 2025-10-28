using Models;
using System;
using System.Collections.Generic;

namespace ViewModels
{
    public class DashboardViewModel
    {
        public string? SearchQuery { get; set; }

        public List<Book> MyBooks { get; set; } = new();
        public List<Club> MyClubs { get; set; } = new();
        public List<Quote> MyQuotes { get; set; } = new();
        public List<Note> MyNotes { get; set; } = new();
        public List<Category> MyCategories { get; set; } = new();

        public List<Notification> Ongoing { get; set; } = new();
        public List<Notification> DueGoing { get; set; } = new();

        public int TotalBooks { get; set; }
        public int TotalClubs { get; set; }
        public int TotalQuotes { get; set; }
        public int TotalNotes { get; set; }
        public int TotalUsers { get; set; }
    }
}
