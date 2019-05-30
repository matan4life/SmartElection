using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class CommiteeAgreement
    {
        public int ElectionId { get; set; }
        public Election Election { get; set; }
        public int ElectoralCommiteeId { get; set; }
        public ElectoralCommitee ElectoralCommitee { get; set; }
        public string CommiteeSign { get; set; }

    }
}
