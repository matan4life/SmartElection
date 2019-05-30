using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Models
{
    public class UserFingers
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string IoTId { get; set; }
        public IoT IoT { get; set; }
        public int Field { get; set; }

    }
}
