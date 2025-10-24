using Enums;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class User : IdentityUser
    {
        public UserRoles Role;
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<Goal> Goals { get; set; }
        public ICollection<Quote> Quotes { get; set; }
        public ICollection<Club> Clubs { get; set; }
        public ICollection<Book> Books { get; set; }

    }
}