using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class IoT
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public int ElectoralCommiteeId { get; set; }
        public ElectoralCommitee ElectoralCommitee { get; set; }

        public List<UserFingers> UserFingers { get; set; } = new List<UserFingers>();
    }
}
