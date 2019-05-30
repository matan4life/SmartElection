using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class ElectoralCommitee
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string AdditionalInformation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<IoT> IoTs { get; set; } = new List<IoT>();
        public List<CommiteeAgreement> CommiteeAgreements { get; set; } = new List<CommiteeAgreement>();
        public List<Turnout> Turnouts { get; set; } = new List<Turnout>();

        public List<UserCertificates> UserCertificates { get; set; } = new List<UserCertificates>();
    }

}
