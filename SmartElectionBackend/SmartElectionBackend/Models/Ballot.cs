using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class Ballot
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public int AlternativeId { get; set; }
        public Alternative Alternative { get; set; }
        public int ElectoralCommiteeId { get; set; }
        public ElectoralCommitee ElectoralCommitee { get; set; }
        public string CommiteeSignature { get; set; }

    }
}
