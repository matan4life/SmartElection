using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class Turnout
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int ElectionId { get; set; }
        public Election Election { get; set; }
        public int ElectoralCommiteeId { get; set; }
        public ElectoralCommitee ElectoralCommitee { get; set; }
        public DateTimeOffset Date { get; set; }
        public string UserSignature { get; set; }
        public string ElectoralCommiteeSignature { get; set; }

    }
}
