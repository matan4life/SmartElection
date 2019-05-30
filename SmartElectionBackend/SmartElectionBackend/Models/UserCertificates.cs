using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class UserCertificates
    {
        public string Id { get; set; }
        public string SubjectName { get; set; }
        public string IssuerName { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int? ElectionId { get; set; }
        public Election Election { get; set; }

        public int? ElectoralCommiteeId { get; set; }
        public ElectoralCommitee ElectoralCommitee { get; set; }
    }
}
