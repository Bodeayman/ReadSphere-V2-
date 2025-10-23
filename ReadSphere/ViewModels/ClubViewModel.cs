using Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class ClubViewModel
    {
        public List<Club> Clubs { get; set; } = new();
        public int Count { get; set; }
        public int ClubId { get; set; }
    }
}
