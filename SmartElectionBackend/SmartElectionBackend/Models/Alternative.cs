using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class Alternative
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }

        public int ElectionId { get; set; }
        public Election Election { get; set; }

        public List<Ballot> Ballots { get; set; } = new List<Ballot>();
    }
}
