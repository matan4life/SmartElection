using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class User : IdentityUser
    {
        public string Country { get; set; }

        public List<Election> Elections { get; set; } = new List<Election>();
        public List<Turnout> Turnouts { get; set; } = new List<Turnout>();
        public List<UserCertificates> UserCertificates { get; set; } = new List<UserCertificates>();
        public List<UserFingers> UserFingers { get; set; } = new List<UserFingers>();
    }
}
