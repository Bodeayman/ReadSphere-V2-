using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ReadSphere.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Note> Notes { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Category> Categories { get; set; }

    }


}
