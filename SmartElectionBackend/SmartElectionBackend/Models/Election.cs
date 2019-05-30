using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class Election
    {
        public int Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public bool NeedsFingerprint { get; set; }
        public bool NeedsCertificate { get; set; }
        public string CalculationType { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public List<Turnout> Turnouts { get; set; } = new List<Turnout>();

        public List<Alternative> Alternatives { get; set; } = new List<Alternative>();
        public List<CommiteeAgreement> CommiteeAgreements { get; set; } = new List<CommiteeAgreement>();

        public UserCertificates UserCertificate { get; set; }
    }
}
